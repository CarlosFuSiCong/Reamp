# Demo Request Email Setup

This feature allows users to request demo access directly from the showcase page. When a user submits the form, an email is sent to `sicong.fu@outlook.com` using Resend.

## Setup Instructions

### 1. Create a Resend Account

1. Go to [https://resend.com](https://resend.com)
2. Sign up for a free account
3. Verify your email address

### 2. Get Your API Key

1. Log in to Resend dashboard
2. Navigate to **API Keys** section
3. Click **Create API Key**
4. Name it (e.g., "Reamp Demo Requests")
5. Copy the generated API key

### 3. Configure Environment Variables

1. Create or update `.env.local` file in the `frontend` directory:

```bash
RESEND_API_KEY=re_your_actual_api_key_here
```

2. Never commit this file to git (already in `.gitignore`)

### 4. Verify Domain (Optional - For Production)

For production use, you should verify your domain:

1. In Resend dashboard, go to **Domains**
2. Add your domain (e.g., `reamp.com`)
3. Add the provided DNS records to your domain registrar
4. Wait for verification
5. Update the `from` field in `src/app/api/demo-request/route.ts`:

```typescript
from: 'Demo Requests <noreply@yourdomain.com>',
```

### 5. Test the Feature

1. Start the development server: `pnpm dev`
2. Navigate to `http://localhost:3000/showcase`
3. Scroll to the bottom and click "Request Demo Access"
4. Fill out the form and submit
5. Check `sicong.fu@outlook.com` for the email

## Development Notes

- In development, Resend allows you to send emails from `onboarding@resend.dev`
- This has some limitations (e.g., only to verified emails in free tier)
- For testing, you may want to temporarily change the recipient email to your own verified email

## Free Tier Limits

Resend free tier includes:
- 100 emails per day
- 1 verified domain
- Email sending from `onboarding@resend.dev`

This should be sufficient for demo requests on the showcase page.

## Files Modified

- `src/app/api/demo-request/route.ts` - API endpoint handler
- `src/components/shared/demo-request-dialog.tsx` - Form dialog component
- `src/app/(public)/showcase/page.tsx` - Showcase page
- `src/app/(public)/showcase/showcase-client.tsx` - Client component for CTA section
- `frontend/env.example` - Added RESEND_API_KEY example

## Email Template

The email sent includes:
- Requester's name
- Requester's email
- Selected role (Agent/Studio/Both)
- Company name (optional)
- Additional message (optional)

The email is formatted in HTML with a clean, professional design.
