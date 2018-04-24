using System;
using elephant.core.exception;
using elephant.core.model.p2p.message;
using elephant.core.service;
using elephant.core.vocabulary;
using elephant.validator.service;
using elephant.web.Services.p2p;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace elephant.web.Controllers
{
    [Produces("application/json")]
    public class NodeController : Controller
    {
        private ILogger<NodeController> _logger;
        private BlockService _blockService;
        private PeerToPeerService _peerToPeerService;
        private ValidationService _validationService;

        public NodeController(ILogger<NodeController> logger, BlockService blockService, PeerToPeerService peerToPeerService, ValidationService validationService)
        {
            _logger = logger;
            _blockService = blockService;
            _peerToPeerService = peerToPeerService;
            _validationService = validationService;
        }

        [HttpPost]
        [Route("api/block/create")]
        public IActionResult CreateBlock([FromQuery] string graphIri, string rdfGraphContent)
        {
            _logger.LogDebug("[POST] block/create ? graphIri={}", graphIri);
            _logger.LogTrace("<rdfGraphContent>\n{}\n</rdfGraphContent>", rdfGraphContent);
            try
            {
                var block = _blockService.CreateBlock(graphIri, rdfGraphContent, RdfFormat.TURTLE);
                _logger.LogDebug("Block created successfully. Broadcasting change.");
                _logger.LogTrace("\tCreated block details: {}", block);
                _peerToPeerService.BroadcastBlockCreation(MessageType.RESPONSE_BLOCKCHAIN, block);
                return Ok(block.BlockHeader);
            }
            catch (CreatingBlockException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("api/block")]
        public IActionResult GetAllBlocks()
        {
            _logger.LogDebug("[GET] block");
            try
            {
                var allBlocks = _blockService.GetAllBlocks();
                return Ok(allBlocks);
            }
            catch (ReadingBlockException ex)
            {
                _logger.LogWarning("Exception was thrown while getting all blocks.", ex);
                return NoContent();
            }
        }

        [HttpGet]
        [Route("api/block/{blockIndex}")]
        public IActionResult GetBlock(string blockIndex)
        {
            _logger.LogDebug("[GET] block/{}", blockIndex);
            try
            {
                var block = _blockService.GetBlock(blockIndex);
                return Ok(block.BlockHeader);
            }
            catch (BlockNotFoundException)
            {
                return NotFound();
            }
            catch (ReadingBlockException ex)
            {
                var msg = String.Format("Exception was thrown while getting the block with index '{0}'.", blockIndex);
                _logger.LogWarning(msg, ex);
                return NotFound();
            }
        }

        [HttpGet]
        [Route("api/validate")]
        public IActionResult Validate()
        {
            _logger.LogDebug("[GET] validate");
            var result = _validationService.ValidateGraphChain();
            return Ok(result.IsValid);
        }
    }
}