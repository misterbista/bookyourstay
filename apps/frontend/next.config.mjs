/** @type {import('next').NextConfig} */
const nextConfig = {
  transpilePackages: ["@bookyourstay/shared"],
  images: {
    remotePatterns: [
      {
        protocol: "https",
        hostname: "images.unsplash.com",
      },
    ],
  },
}

export default nextConfig
