using System;
using System.Collections.Generic;
using LircSharp.DAL;
using LircSharp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace PiIRRemote.Controllers
{
    [Route("api/[controller]")]
    public class RemotesController : Controller
    {
        [HttpGet]
        public ActionResult<List<Remote>> GetRemotes()
        {
            DAL dal = new DAL();
            try
            {
                List<Remote> remotes = dal.GetRemotes();
                return Ok(remotes);
            }
            catch (NotSupportedException e)
            {
                return StatusCode(501, e.Message);
            }
        }

        [Route("{remoteName}")]
        [HttpGet]
        public ActionResult<Remote> GetRemote(string remoteName)
        {
            DAL dal = new DAL();
            try
            {
                Remote remote = dal.GetRemote(remoteName);

                if (remote == null)
                {
                    return NotFound();
                }

                return Ok(remote);
            }
            catch (NotSupportedException e)
            {
                return StatusCode(501, e.Message);
            }
        }

        [Route("{remoteName}/commands")]
        [HttpGet]
        public ActionResult<List<RemoteCode>> GetRemoteCodes(string remoteName)
        {
            DAL dal = new DAL();
            try
            {
                Remote remote = dal.GetRemote(remoteName);
                List<RemoteCode> codes = dal.GetRemoteCodes(remote);

                return Ok(codes);
            }
            catch (NotSupportedException e)
            {
                return StatusCode(401, e.Message);
            }
        }

        [Route("{remoteName}/commands/{commandName}")]
        [HttpPost]
        public ActionResult Post(string remoteName, string commandName)
        {
            DAL dal = new DAL();
            try
            {
                Remote remote = dal.GetRemote(remoteName);

                if(remote == null)
                {
                    return StatusCode(404, $"Remote {remoteName} not found");
                }

                RemoteCode code = remote.Codes.FirstOrDefault(code => code.Name == commandName);

                if(code == null)
                {
                    return StatusCode(404, $"Command {commandName} not found");
                }

                dal.SendRemoteCode(code);

                return Ok();
            }
            catch (NotSupportedException e)
            {
                return StatusCode(501, e.Message);
            }
        }
    }
}
