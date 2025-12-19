import { Resend } from 'resend';
import { NextResponse } from 'next/server';

const resend = new Resend(process.env.RESEND_API_KEY);

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

    // Send email notification
    const { data, error } = await resend.emails.send({
      from: 'Reamp Demo Requests <onboarding@resend.dev>',
      to: ['sicong.fu@outlook.com'],
      subject: `New Demo Access Request from ${name}`,
      html: `
        <div style="font-family: sans-serif; max-width: 600px; margin: 0 auto;">
          <h2 style="color: #2563eb;">New Demo Access Request</h2>
          
          <div style="background: #f3f4f6; padding: 20px; border-radius: 8px; margin: 20px 0;">
            <p style="margin: 10px 0;"><strong>Name:</strong> ${name}</p>
            <p style="margin: 10px 0;"><strong>Email:</strong> ${email}</p>
            <p style="margin: 10px 0;"><strong>Role:</strong> ${role}</p>
            ${company ? `<p style="margin: 10px 0;"><strong>Company:</strong> ${company}</p>` : ''}
          </div>
          
          ${message ? `
            <div style="margin: 20px 0;">
              <strong>Message:</strong>
              <p style="background: #f9fafb; padding: 15px; border-radius: 8px; border-left: 4px solid #2563eb;">
                ${message}
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
