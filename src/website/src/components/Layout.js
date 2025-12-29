import * as React from "react"
import { Link } from "gatsby"
import "../styles/global.css"

const Layout = ({ children, pageTitle }) => {
  return (
    <>
      <header>
        <div className="container">
          <h1>
            <Link to="/" style={{ color: "white", textDecoration: "none" }}>
              🔒 Ad-Blocking Toolkit
            </Link>
          </h1>
        </div>
      </header>
      <nav>
        <div className="container">
          <ul>
            <li>
              <Link to="/">Home</Link>
            </li>
            <li>
              <Link to="/getting-started">Getting Started</Link>
            </li>
            <li>
              <Link to="/docs">Documentation</Link>
            </li>
            <li>
              <Link to="/guides">Guides</Link>
            </li>
            <li>
              <Link to="/api">API Reference</Link>
            </li>
            <li>
              <Link to="/improvements">Recent Improvements</Link>
            </li>
          </ul>
        </div>
      </nav>
      <main>
        <div className="container">
          {pageTitle && <h1>{pageTitle}</h1>}
          {children}
        </div>
      </main>
      <footer>
        <div className="container">
          <p>
            Ad-Blocking Toolkit - Licensed under{" "}
            <a href="https://github.com/jaypatrick/ad-blocking/blob/main/LICENSE">
              GPL-3.0
            </a>
          </p>
          <p>
            <a href="https://github.com/jaypatrick/ad-blocking">
              View on GitHub
            </a>
          </p>
        </div>
      </footer>
    </>
  )
}

export default Layout
