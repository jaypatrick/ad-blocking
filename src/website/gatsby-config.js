/**
 * Gatsby configuration for Ad-Blocking documentation website
 * @type {import('gatsby').GatsbyConfig}
 */
module.exports = {
  siteMetadata: {
    title: `Ad-Blocking Toolkit`,
    description: `A comprehensive multi-language toolkit for ad-blocking, network protection, and AdGuard DNS management`,
    author: `Ad-Blocking Contributors`,
    siteUrl: `https://jaypatrick.github.io/ad-blocking`,
  },
  pathPrefix: `/ad-blocking`,
  plugins: [
    `gatsby-plugin-image`,
    `gatsby-plugin-sharp`,
    `gatsby-transformer-sharp`,
    {
      resolve: `gatsby-source-filesystem`,
      options: {
        name: `docs`,
        path: `${__dirname}/../../docs`,
      },
    },
    {
      resolve: `gatsby-source-filesystem`,
      options: {
        name: `root-docs`,
        path: `${__dirname}/../..`,
        ignore: [`**/node_modules/**`, `**/.*`, `**/data/**`, `**/src/**`, `**/.github/**`, `**/tools/**`, `**/api/**`],
      },
    },
    {
      resolve: `gatsby-transformer-remark`,
      options: {
        plugins: [],
      },
    },
    {
      resolve: `gatsby-plugin-manifest`,
      options: {
        name: `Ad-Blocking Toolkit`,
        short_name: `Ad-Blocking`,
        start_url: `/`,
        background_color: `#663399`,
        theme_color: `#663399`,
        display: `minimal-ui`,
        icon: `src/images/icon.svg`,
      },
    },
  ],
}
