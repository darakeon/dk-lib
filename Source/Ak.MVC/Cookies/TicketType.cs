namespace Ak.MVC.Cookies
{
    /// <summary>
    /// Cookie ticket type
    /// </summary>
    public enum TicketType
    {
        /// <summary>
        /// Web
        /// </summary>
        Browser = 0,
        
        /// <summary>
        /// API Call
        /// </summary>
        Cellphone = 1,

        /// <summary>
        /// Desktop or tests
        /// </summary>
        Local = 2,
    }
}
