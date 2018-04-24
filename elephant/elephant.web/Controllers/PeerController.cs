using System;
using elephant.core.model.p2p.message;
using elephant.web.Services.p2p;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace elephant.web.Controllers
{
    [Produces("application/json")]
    [Route("api/peer")]
    public class PeerController : Controller
    {
        private ILogger<PeerController> _logger;
        private PeerToPeerService _peerToPeerService;

        public PeerController(ILogger<PeerController> logger, PeerToPeerService peerToPeerService)
        {
            _logger = logger;
            _peerToPeerService = peerToPeerService;
        }

        [HttpPost]
        [Route("add")]
        public IActionResult AddPeer([FromBody] AddPeerMessage messageInJson)
        {
            _logger.LogDebug("[POST] peer/add");
            _logger.LogTrace("@RequestBody:\n<message>\n{}\n</message>", messageInJson);
            if (!messageInJson.IsMessageValid())
            {
                return BadRequest("Message is not valid");
            }
            var success = _peerToPeerService.AddPeer(messageInJson.peerAddress);
            if (success)
            {
                return Accepted("Added peer");
            }
            else
            {
                return BadRequest("Unable to connect to a peer.");
            }
        }

        [HttpGet]
        [Route("")]
        public IActionResult GetPeers()
        {
            var peers = _peerToPeerService.GetAllPeers();
            return Ok(peers);
        }

        [HttpGet]
        [Route("broadcast")]
        public IActionResult Broadcast(string messageType)
        {
            _logger.LogDebug("[GET] peer/broadcast ? messageType = {0}", messageType);
            _peerToPeerService.Broadcast((MessageType)Enum.Parse(typeof(MessageType), messageType));
            return Accepted();
        }
    }
}