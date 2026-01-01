import * as React from "react"
import { Link } from "gatsby"
import Layout from "../components/Layout"

const AdBlockCompilerPage = () => {
  return (
    <Layout pageTitle="AdBlock Compiler">
      <p style={{ fontSize: "1.1rem", marginBottom: "2rem" }}>
        A modern, TypeScript-based compiler-as-a-service for adblock filter lists with 
        real-time progress tracking, visual diff, and production-ready features.
      </p>

      <section>
        <h2>Overview</h2>
        <p>
          AdBlock Compiler is a Deno-native rewrite of the original @adguard/hostlist-compiler
          with significant improvements for production use. It provides the same functionality
          with enhanced performance, Cloudflare Workers support, and a comprehensive API.
        </p>
      </section>

      <section style={{ marginTop: "2rem" }}>
        <h2>✨ Key Features</h2>
        <div className="features">
          <div className="feature">
            <h3>🎯 Multi-Source Compilation</h3>
            <p>
              Combine filter lists from URLs, files, or inline rules with automatic
              format detection and validation.
            </p>
          </div>
          <div className="feature">
            <h3>⚡ Performance Optimizations</h3>
            <p>
              Gzip compression (70-80% size reduction), request deduplication,
              smart caching with 1-hour TTL.
            </p>
          </div>
          <div className="feature">
            <h3>🔄 Circuit Breaker</h3>
            <p>
              Automatic retry with exponential backoff for unreliable sources
              (3 retries, smart error handling).
            </p>
          </div>
          <div className="feature">
            <h3>📊 Visual Diff</h3>
            <p>
              See what changed between compilations with color-coded diffs showing
              added, removed, and unchanged rules.
            </p>
          </div>
          <div className="feature">
            <h3>🎪 Batch Processing</h3>
            <p>
              Compile up to 10 filter lists in parallel with a single API request.
            </p>
          </div>
          <div className="feature">
            <h3>📡 Event Pipeline</h3>
            <p>
              Real-time progress tracking via Server-Sent Events (SSE) with
              detailed compilation stages.
            </p>
          </div>
          <div className="feature">
            <h3>🌍 Universal Runtime</h3>
            <p>
              Works in Deno, Node.js, Cloudflare Workers, and browsers with
              zero build configuration.
            </p>
          </div>
          <div className="feature">
            <h3>🎨 11 Transformations</h3>
            <p>
              Deduplicate, compress, validate, remove comments, and more—all
              applied in optimized order.
            </p>
          </div>
        </div>
      </section>

      <section style={{ marginTop: "2rem" }}>
        <h2>🚀 Quick Start</h2>
        
        <h3>Installation</h3>
        <pre style={{ marginTop: "0.5rem", marginBottom: "1.5rem" }}>
          # Install from JSR (recommended)
          <br />
          deno add jsr:@jk-com/adblock-compiler
          <br />
          <br />
          # Or run directly without installation
          <br />
          deno run jsr:@jk-com/adblock-compiler/cli
        </pre>

        <h3>Basic Usage</h3>
        <pre style={{ marginTop: "0.5rem", marginBottom: "1.5rem" }}>
          {`import { compile } from '@jk-com/adblock-compiler';

// Compile from configuration
const rules = await compile({
  name: "My Filter List",
  sources: [
    {
      source: "https://example.com/filters.txt"
    }
  ],
  transformations: ["Deduplicate", "RemoveEmptyLines"]
});

console.log(\`Compiled \${rules.length} rules\`);`}
        </pre>

        <h3>CLI Usage</h3>
        <pre style={{ marginTop: "0.5rem" }}>
          # Compile from config file
          <br />
          deno run jsr:@jk-com/adblock-compiler/cli -c config.yaml
          <br />
          <br />
          # Show version
          <br />
          deno run jsr:@jk-com/adblock-compiler/cli --version
        </pre>
      </section>

      <section style={{ marginTop: "2rem" }}>
        <h2>🌐 Web Interface & API</h2>
        <p>
          AdBlock Compiler is deployed as a production service with a full-featured
          web interface and REST API.
        </p>

        <div className="features">
          <div className="feature">
            <h3>
              <a href="https://adblock.jaysonknight.com" target="_blank" rel="noopener noreferrer">
                🌐 Web UI
              </a>
            </h3>
            <p>
              Interactive web interface with Simple Mode, Advanced Mode, examples,
              and real-time compilation progress.
            </p>
          </div>
          <div className="feature">
            <h3>
              <a href="https://adblock-compiler.jayson-knight.workers.dev/api" target="_blank" rel="noopener noreferrer">
                🚀 API Endpoint
              </a>
            </h3>
            <p>
              RESTful API with JSON responses, SSE streaming, and batch processing
              endpoints.
            </p>
          </div>
        </div>

        <h3 style={{ marginTop: "1.5rem" }}>API Endpoints</h3>
        <ul style={{ fontSize: "1rem", lineHeight: "1.8" }}>
          <li><strong>GET /api</strong> - API information and examples</li>
          <li><strong>POST /compile</strong> - Compile with JSON response</li>
          <li><strong>POST /compile/stream</strong> - Compile with SSE progress</li>
          <li><strong>POST /compile/batch</strong> - Compile multiple lists in parallel</li>
        </ul>

        <h3 style={{ marginTop: "1.5rem" }}>Example API Request</h3>
        <pre style={{ marginTop: "0.5rem" }}>
          {`curl -X POST https://adblock-compiler.jayson-knight.workers.dev/compile \\
  -H "Content-Type: application/json" \\
  -d '{
    "configuration": {
      "name": "My Filter List",
      "sources": [
        {"source": "https://example.com/filters.txt"}
      ],
      "transformations": ["Deduplicate", "RemoveEmptyLines"]
    }
  }'`}
        </pre>
      </section>

      <section style={{ marginTop: "2rem" }}>
        <h2>📊 Performance Features</h2>
        
        <h3>Caching with Compression</h3>
        <p>
          Compilation results are cached with gzip compression for 1 hour. Cache is
          automatically invalidated when configuration changes. Compression typically
          achieves 70-80% size reduction.
        </p>

        <h3>Request Deduplication</h3>
        <p>
          Identical concurrent requests are automatically deduplicated. Only one
          compilation executes, with all requests receiving the same result.
          Check the <code>X-Request-Deduplication: HIT</code> header.
        </p>

        <h3>Circuit Breaker</h3>
        <p>External source downloads include:</p>
        <ul>
          <li><strong>Timeout:</strong> 30 seconds per request</li>
          <li><strong>Retry Logic:</strong> Up to 3 retries with exponential backoff</li>
          <li><strong>Retry Conditions:</strong> 5xx errors, 429 rate limits, timeouts</li>
          <li><strong>No Retry:</strong> 4xx client errors (except 429)</li>
        </ul>

        <h3>Rate Limiting</h3>
        <ul>
          <li><strong>Limit:</strong> 10 requests per minute per IP address</li>
          <li><strong>Response:</strong> HTTP 429 with <code>Retry-After: 60</code> header</li>
          <li><strong>Batch Limit:</strong> Maximum 10 compilations per batch request</li>
        </ul>
      </section>

      <section style={{ marginTop: "2rem" }}>
        <h2>🔧 Configuration</h2>
        <p>
          AdBlock Compiler supports the same configuration schema as the original
          hostlist-compiler, ensuring compatibility with existing configurations.
        </p>

        <h3>Configuration Example (YAML)</h3>
        <pre style={{ marginTop: "0.5rem" }}>
          {`name: My Filter List
description: Custom ad-blocking filter
version: "1.0.0"

sources:
  - name: Local Rules
    source: data/local.txt
    type: adblock

  - name: EasyList
    source: https://easylist.to/easylist/easylist.txt
    transformations:
      - RemoveModifiers
      - Validate

transformations:
  - Deduplicate
  - RemoveEmptyLines
  - InsertFinalNewLine

exclusions:
  - "*.google.com"
  - "/analytics/"`}
        </pre>

        <h3 style={{ marginTop: "1.5rem" }}>Available Transformations</h3>
        <ol style={{ lineHeight: "1.8" }}>
          <li><strong>ConvertToAscii</strong> - Convert internationalized domains</li>
          <li><strong>TrimLines</strong> - Remove whitespace</li>
          <li><strong>RemoveComments</strong> - Strip comments</li>
          <li><strong>Compress</strong> - Convert hosts to adblock syntax</li>
          <li><strong>RemoveModifiers</strong> - Remove unsupported modifiers</li>
          <li><strong>InvertAllow</strong> - Convert to allowlist</li>
          <li><strong>Validate</strong> - Remove invalid rules</li>
          <li><strong>ValidateAllowIp</strong> - Validate but keep IPs</li>
          <li><strong>Deduplicate</strong> - Remove duplicates</li>
          <li><strong>RemoveEmptyLines</strong> - Clean empty lines</li>
          <li><strong>InsertFinalNewLine</strong> - Add final newline</li>
        </ol>
      </section>

      <section style={{ marginTop: "2rem" }}>
        <h2>📚 Documentation & Resources</h2>
        <div className="features">
          <div className="feature">
            <h3>
              <a href="https://github.com/jaypatrick/adblock-compiler" target="_blank" rel="noopener noreferrer">
                GitHub Repository
              </a>
            </h3>
            <p>
              Source code, issue tracker, and contribution guidelines.
            </p>
          </div>
          <div className="feature">
            <h3>
              <a href="https://github.com/jaypatrick/adblock-compiler/tree/master/docs/api" target="_blank" rel="noopener noreferrer">
                API Documentation
              </a>
            </h3>
            <p>
              Complete API reference with examples and client code.
            </p>
          </div>
          <div className="feature">
            <h3>
              <a href="https://github.com/jaypatrick/adblock-compiler/tree/master/docs/guides" target="_blank" rel="noopener noreferrer">
                Client Libraries
              </a>
            </h3>
            <p>
              Python, TypeScript/JavaScript, and Go client implementations.
            </p>
          </div>
        </div>
      </section>

      <section style={{ marginTop: "2rem" }}>
        <h2>🆚 Comparison with Original</h2>
        <p>
          AdBlock Compiler is a drop-in replacement for @adguard/hostlist-compiler
          with additional features:
        </p>
        
        <table style={{ width: "100%", marginTop: "1rem", borderCollapse: "collapse" }}>
          <thead>
            <tr style={{ borderBottom: "2px solid #ccc" }}>
              <th style={{ padding: "0.75rem", textAlign: "left" }}>Feature</th>
              <th style={{ padding: "0.75rem", textAlign: "center" }}>Original</th>
              <th style={{ padding: "0.75rem", textAlign: "center" }}>AdBlock Compiler</th>
            </tr>
          </thead>
          <tbody>
            <tr style={{ borderBottom: "1px solid #eee" }}>
              <td style={{ padding: "0.75rem" }}>Core Compilation</td>
              <td style={{ padding: "0.75rem", textAlign: "center" }}>✅</td>
              <td style={{ padding: "0.75rem", textAlign: "center" }}>✅</td>
            </tr>
            <tr style={{ borderBottom: "1px solid #eee" }}>
              <td style={{ padding: "0.75rem" }}>11 Transformations</td>
              <td style={{ padding: "0.75rem", textAlign: "center" }}>✅</td>
              <td style={{ padding: "0.75rem", textAlign: "center" }}>✅</td>
            </tr>
            <tr style={{ borderBottom: "1px solid #eee" }}>
              <td style={{ padding: "0.75rem" }}>Cloudflare Workers</td>
              <td style={{ padding: "0.75rem", textAlign: "center" }}>❌</td>
              <td style={{ padding: "0.75rem", textAlign: "center" }}>✅</td>
            </tr>
            <tr style={{ borderBottom: "1px solid #eee" }}>
              <td style={{ padding: "0.75rem" }}>Gzip Compression</td>
              <td style={{ padding: "0.75rem", textAlign: "center" }}>❌</td>
              <td style={{ padding: "0.75rem", textAlign: "center" }}>✅</td>
            </tr>
            <tr style={{ borderBottom: "1px solid #eee" }}>
              <td style={{ padding: "0.75rem" }}>Request Deduplication</td>
              <td style={{ padding: "0.75rem", textAlign: "center" }}>❌</td>
              <td style={{ padding: "0.75rem", textAlign: "center" }}>✅</td>
            </tr>
            <tr style={{ borderBottom: "1px solid #eee" }}>
              <td style={{ padding: "0.75rem" }}>Circuit Breaker</td>
              <td style={{ padding: "0.75rem", textAlign: "center" }}>❌</td>
              <td style={{ padding: "0.75rem", textAlign: "center" }}>✅</td>
            </tr>
            <tr style={{ borderBottom: "1px solid #eee" }}>
              <td style={{ padding: "0.75rem" }}>Visual Diff</td>
              <td style={{ padding: "0.75rem", textAlign: "center" }}>❌</td>
              <td style={{ padding: "0.75rem", textAlign: "center" }}>✅</td>
            </tr>
            <tr style={{ borderBottom: "1px solid #eee" }}>
              <td style={{ padding: "0.75rem" }}>Batch Processing API</td>
              <td style={{ padding: "0.75rem", textAlign: "center" }}>❌</td>
              <td style={{ padding: "0.75rem", textAlign: "center" }}>✅</td>
            </tr>
            <tr style={{ borderBottom: "1px solid #eee" }}>
              <td style={{ padding: "0.75rem" }}>SSE Event Pipeline</td>
              <td style={{ padding: "0.75rem", textAlign: "center" }}>❌</td>
              <td style={{ padding: "0.75rem", textAlign: "center" }}>✅</td>
            </tr>
            <tr>
              <td style={{ padding: "0.75rem" }}>Web UI</td>
              <td style={{ padding: "0.75rem", textAlign: "center" }}>❌</td>
              <td style={{ padding: "0.75rem", textAlign: "center" }}>✅</td>
            </tr>
          </tbody>
        </table>
      </section>

      <section style={{ marginTop: "2rem" }}>
        <h2>Next Steps</h2>
        <div className="features">
          <div className="feature">
            <h3>
              <a href="https://adblock.jaysonknight.com" target="_blank" rel="noopener noreferrer">
                Try the Web UI
              </a>
            </h3>
            <p>
              Test the compiler with the interactive web interface.
            </p>
          </div>
          <div className="feature">
            <h3>
              <Link to="/getting-started">Getting Started</Link>
            </h3>
            <p>
              Learn how to integrate AdBlock Compiler into your workflow.
            </p>
          </div>
          <div className="feature">
            <h3>
              <a href="https://github.com/jaypatrick/adblock-compiler" target="_blank" rel="noopener noreferrer">
                View on GitHub
              </a>
            </h3>
            <p>
              Explore the source code and contribute to development.
            </p>
          </div>
        </div>
      </section>
    </Layout>
  )
}

export default AdBlockCompilerPage

export const Head = () => <title>AdBlock Compiler - AdGuard Tools and Utilities</title>
