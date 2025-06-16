using AutoMapper;
using AutoMapper.QueryableExtensions;
using DatingApp_API.Entities;
using DatingApp_API.Helpers;
using DatingApp_API.Interfaces;
using DatingApp_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp_API.Data
{
    public class MessageRepository(DataContext context, IMapper mapper) : IMessageRepository
    {
        private readonly DataContext _context = context;
        private readonly IMapper _mapper = mapper;

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }
         
        public async Task<Message?> GetMessage(int id)
        {
            return await _context.Messages.FindAsync(id);
        }

        public async Task<PagedList<MessageDTO>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = _context.Messages
                .OrderByDescending(x => x.DateSent)
                .AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(x => x.Recipient.UserName == messageParams.Username && x.RecipientDeleted == false),
                "Outbox" => query.Where(x => x.Sender.UserName == messageParams.Username && x.SenderDeleted == false),
                _ => query.Where(x => x.Recipient.UserName == messageParams.Username && x.DateRead == null && x.RecipientDeleted == false),
            };

            var messages = query.ProjectTo<MessageDTO>(_mapper.ConfigurationProvider);

            return await PagedList<MessageDTO>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUsername, string recipientUsername)
        {
           var messages = await _context.Messages
                .Include(x => x.Sender)
                .ThenInclude(x => x.Photos) 
                .Include(x => x.Recipient)
                .ThenInclude(x => x.Photos)
                .Where(x => x.SenderUsername == currentUsername && x.RecipientUsername == recipientUsername && x.SenderDeleted == false
                 || x.RecipientUsername == currentUsername && x.SenderUsername == recipientUsername && x.RecipientDeleted == false)
                .OrderBy(x => x.DateSent)
                .ToListAsync();

            var unreadMessages = messages.Where(x => x.DateRead == null && x.RecipientUsername == currentUsername).ToList();

            if(unreadMessages.Count != 0)
            {
                unreadMessages.ForEach(x => x.DateRead = DateTime.UtcNow);
            }

            return _mapper.Map<IEnumerable<MessageDTO>>(messages);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
