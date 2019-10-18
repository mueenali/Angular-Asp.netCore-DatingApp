using System;

namespace DatingApp.API.Dtos
{
    public class MessageCreateDto
    {

        public int SenderId { get; set; }

        public int ReceiverId { get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; }
        public DateTime DateSent { get; set; }

        public MessageCreateDto()
        {
            this.DateSent = DateTime.Now;
        }

    }
}