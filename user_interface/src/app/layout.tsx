import type { Metadata } from "next";
import localFont from "next/font/local";
import "./globals.css";


const jetBrainsMono = localFont({
  src: "./fonts/JetBrainsMono_VariableFont.woff2",
  variable: "--font-jb-mono",
  weight: "100 900",
});
const montserrat = localFont({
  src: "./fonts/Montserrat-VariableFont.woff2",
  variable: "--font-montserrat-sans",
  weight: "100 900",
});

export const metadata: Metadata = {
  title: "Mirage",
  description: "A Map Tool For MIR robots",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en">
      <body
        className={`${montserrat.variable} ${jetBrainsMono.variable} antialiased`}
      >
        {children}
      </body>
    </html>
  );
}
