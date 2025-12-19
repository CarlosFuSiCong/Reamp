import { Resend } from 'resend';
import { NextResponse } from 'next/server';

const resend = new Resend(process.env.RESEND_API_KEY);

/**
 * Escapes HTML entities to prevent XSS/HTML injection
 */
function escapeHtml(text: string): string {
  const htmlEntities: Record<string, string> = {
    '&': '&amp;',
    '<': '&lt;',
    '>': '&gt;',
    '"': '&quot;',
    "'": '&#39;',
    '/': '&#x2F;',
  };
  return text.replace(/[&<>"'/]/g, (char) => htmlEntities[char]);
}

export async function POST(request: Request) {
  try {
    const { name, email, company, role, message } = await request.json();

    // Validate required fields
    if (!name || !email || !role) {
      return NextResponse.json(
        { error: 'Name, email, and role are required' },
        { status: 400 }
      );
    }

    // Escape all user inputs to prevent HTML/XSS injection
    const safeName = escapeHtml(String(name));
    const safeEmail = escapeHtml(String(email));
    const safeRole = escapeHtml(String(role));
    const safeCompany = company ? escapeHtml(String(company)) : '';
    const safeMessage = message ? escapeHtml(String(message)) : '';

    // Get recipient email from environment variable
    const recipientEmail = process.env.DEMO_REQUEST_RECIPIENT_EMAIL || 'sicong.fu@outlook.com';

    // Send email notification
    const { data, error } = await resend.emails.send({
      from: 'Reamp Demo Requests <onboarding@resend.dev>',
      to: [recipientEmail],
      subject: `New Demo Access Request from ${name}`,
      html: `
        <div style="font-family: sans-serif; max-width: 600px; margin: 0 auto;">
          <h2 style="color: #2563eb;">New Demo Access Request</h2>
          
          <div style="background: #f3f4f6; padding: 20px; border-radius: 8px; margin: 20px 0;">
            <p style="margin: 10px 0;"><strong>Name:</strong> ${safeName}</p>
            <p style="margin: 10px 0;"><strong>Email:</strong> ${safeEmail}</p>
            <p style="margin: 10px 0;"><strong>Role:</strong> ${safeRole}</p>
            ${safeCompany ? `<p style="margin: 10px 0;"><strong>Company:</strong> ${safeCompany}</p>` : ''}
          </div>
          
          ${safeMessage ? `
            <div style="margin: 20px 0;">
              <strong>Message:</strong>
              <p style="background: #f9fafb; padding: 15px; border-radius: 8px; border-left: 4px solid #2563eb; white-space: pre-wrap;">
                ${safeMessage}
              </p>
            </div>
          ` : ''}
          
          <hr style="margin: 30px 0; border: none; border-top: 1px solid #e5e7eb;">
          
          <p style="color: #6b7280; font-size: 14px;">
            This email was sent from the Reamp showcase page demo request form.
          </p>
        </div>
      `,
    });

    if (error) {
      console.error('Resend error:', error);
      return NextResponse.json(
        { error: 'Failed to send email' },
        { status: 500 }
      );
    }

    return NextResponse.json({ success: true, data });
  } catch (error) {
    console.error('Server error:', error);
    return NextResponse.json(
      { error: 'Internal server error' },
      { status: 500 }
    );
  }
}
