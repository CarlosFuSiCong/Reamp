# Demo Request Setup

## Quick Setup

1. Sign up at [https://resend.com](https://resend.com) (free)
2. Get your API key from the dashboard
3. Add to `frontend/.env.local`:
   ```
   RESEND_API_KEY=re_your_key_here
   ```
4. Restart dev server

That's it! The "Request Demo Access" button on `/showcase` will now send emails to `sicong.fu@outlook.com`.
