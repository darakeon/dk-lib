using System;

namespace DK.MVC.Cookies
{
    /// <summary>
    /// Ticket for cookie
    /// </summary>
    public class PseudoTicket
    {
        internal PseudoTicket(String key, TicketType type)
        {
            Key = key;
            Type = type;
        }

        /// <summary>
        /// Name
        /// </summary>
        public String Key { get; private set; }
        
        /// <summary>
        /// Cellphone, browser or local
        /// </summary>
        public TicketType Type { get; private set; }

    }
}
