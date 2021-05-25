using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using RazorPartialToString.Services;
using ToSic.Sxc.Razor;

namespace Website.Pages
{
    public class ContactModel : PageModel
    {
        private readonly IRazorRenderer _renderer;
        public ContactModel(IRazorRenderer renderer)
        {
            _renderer = renderer;
        }
        [BindProperty]
        public ContactForm ContactForm { get; set; }
        [TempData]
        public string PostResult { get; set; }

        public async Task OnGetAsync()
        {
            InnerRender = await _renderer.RenderToStringAsync("RenderPartial/_RenderPartialMail",
                new ContactForm
                {
                    Email = "something@somewhere",
                    Message = "Hello message!",
                    Name = "The Dude",
                    Priority = Priority.Medium,
                    Subject = "This is the subject"
                });
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var body = await _renderer.RenderToStringAsync("RenderPartial/_RenderPartialMail", ContactForm);
            PostResult = body; // "Check your specified pickup directory";
            return RedirectToPage();
        }


        public string InnerRender;
    }
    public class ContactForm
    {
        public string Email { get; set; }
        public string Message { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public Priority Priority { get; set; }
    }
    public enum Priority
    {
        Low, Medium, High
    }
}