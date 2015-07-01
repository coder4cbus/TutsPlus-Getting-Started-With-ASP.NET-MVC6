﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using SoundBoard.Models;
using SoundBoard.Data;
using System.Security.Claims;
using Microsoft.AspNet.Authorization;
using SoundBoard.Services;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace SoundBoard.Controllers
{
    [Authorize]
    public class MessageController : Controller
    {
        private readonly MessageRepository _messages;
        private readonly ViewMessageService _service;

        public MessageController(MessageRepository messages, ApplicationDbContext dbContext)
        {
            _messages = messages;
            _service = new ViewMessageService(messages, dbContext);
        }
        
        // GET: /<controller>/
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var messages = await _service.GetAllMessagesAsync();

            return View(messages);
        }

        // /messsage/create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Message model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.Created = DateTime.Now;
            model.UserId = User.GetUserId();

            _messages.Add(model);

            return RedirectToAction("index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var message = _messages.GetBy(id);

            if (message == null)
            {
                return HttpNotFound();
            }

            if (message.UserId != User.GetUserId())
            {
                return HttpUnauthorized();
            }

            return View(message);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Message model)
        {
            var message = _messages.GetBy(id);

            if (message == null)
            {
                return HttpNotFound();
            }

            if (message.UserId != User.GetUserId())
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            message.MessageTitle = model.MessageTitle;
            message.MessageContent = model.MessageContent;

            _messages.Update(message);


            return RedirectToAction("index");
        }
    }
}
