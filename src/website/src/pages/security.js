import * as React from "react"
import { Link } from "gatsby"
import Layout from "../components/Layout"

const SecurityPage = () => {
  return (
    <Layout pageTitle="Security">
      <div className="hero" style={{ marginBottom: "2rem" }}>
        <h2>🛡️ Your Safety is Our Priority</h2>
        <p style={{ fontSize: "1.2rem" }}>
          Learn how our built-in security features protect you from malicious
          filter lists, tampering, and network attacks.
        </p>
      </div>

      <section style={{ marginBottom: "3rem" }}>
        <h2>Why Security Matters</h2>
        <p style={{ fontSize: "1.1rem", lineHeight: "1.8" }}>
          When you use ad-blocking or DNS filtering, you're trusting filter
          lists to protect your network. But what if those lists themselves
          become compromised? Our <strong>mandatory validation system</strong>{" "}
          ensures that every filter list you use is authentic, unmodified, and
          safe.
        </p>

        <div
          className="features"
          style={{ marginTop: "2rem", marginBottom: "2rem" }}
        >
          <div
            className="feature"
            style={{ backgroundColor: "#f8f9fa", padding: "1.5rem" }}
          >
            <h3>🔒 Automatic Protection</h3>
            <p>
              Security validation runs automatically every time you compile
              rules. No configuration needed - it just works.
            </p>
          </div>
          <div
            className="feature"
            style={{ backgroundColor: "#f8f9fa", padding: "1.5rem" }}
          >
            <h3>⚡ Lightning Fast</h3>
            <p>
              Adds less than 100ms to compilation time. Get enterprise-grade
              security without the wait.
            </p>
          </div>
          <div
            className="feature"
            style={{ backgroundColor: "#f8f9fa", padding: "1.5rem" }}
          >
            <h3>🎯 Zero False Sense of Security</h3>
            <p>
              Unlike optional features that users forget to enable, our
              validation is mandatory by design.
            </p>
          </div>
        </div>
      </section>

      <section style={{ marginBottom: "3rem" }}>
        <h2>What is Hash Verification?</h2>
        <p style={{ fontSize: "1.1rem", lineHeight: "1.8" }}>
          A <strong>hash</strong> is like a unique fingerprint for a file. Even
          changing a single character creates a completely different
          fingerprint. We use <strong>SHA-384</strong>, a cryptographic hash
          algorithm that creates 96-character fingerprints.
        </p>

        <div
          style={{
            backgroundColor: "#f8f9fa",
            padding: "1.5rem",
            borderRadius: "8px",
            marginTop: "1.5rem",
            marginBottom: "1.5rem",
          }}
        >
          <h3>How It Works: A Simple Example</h3>
          <div
            style={{
              fontFamily: "monospace",
              fontSize: "0.95rem",
              marginTop: "1rem",
            }}
          >
            <p>
              <strong>Original file content:</strong>
            </p>
            <pre
              style={{
                backgroundColor: "#fff",
                padding: "1rem",
                borderRadius: "4px",
              }}
            >
              ||example.com^
            </pre>
            <p>
              <strong>SHA-384 Hash:</strong>
            </p>
            <pre
              style={{
                backgroundColor: "#fff",
                padding: "1rem",
                borderRadius: "4px",
                overflowX: "auto",
              }}
            >
              abc123def456789abc123def456789abc123def456789abc123def456789abc123def456789abc123def456789abc123
            </pre>
            <p style={{ marginTop: "1rem" }}>
              <strong>If someone changes just ONE character (l → 1):</strong>
            </p>
            <pre
              style={{
                backgroundColor: "#fff",
                padding: "1rem",
                borderRadius: "4px",
              }}
            >
              ||examp1e.com^
            </pre>
            <p>
              <strong>New SHA-384 Hash (completely different!):</strong>
            </p>
            <pre
              style={{
                backgroundColor: "#fff",
                padding: "1rem",
                borderRadius: "4px",
                overflowX: "auto",
              }}
            >
              xyz789uvw321abc789uvw321xyz789uvw321abc789uvw321xyz789uvw321abc789uvw321xyz789uvw321xyz789
            </pre>
            <p style={{ marginTop: "1rem", fontWeight: "bold", color: "#d9534f" }}>
              ❌ Hash mismatch detected! Compilation stopped to protect you.
            </p>
          </div>
        </div>
      </section>

      <section style={{ marginBottom: "3rem" }}>
        <h2>5 Real-World Threats We Stop</h2>

        <div style={{ marginTop: "2rem" }}>
          <div style={{ marginBottom: "2.5rem" }}>
            <h3>🚨 1. Man-in-the-Middle Attacks</h3>
            <p style={{ fontSize: "1.05rem", lineHeight: "1.7" }}>
              <strong>The Threat:</strong> An attacker intercepts your download
              of a filter list and replaces it with a malicious version that
              redirects banking sites or unblocks malware.
            </p>
            <p style={{ fontSize: "1.05rem", lineHeight: "1.7" }}>
              <strong>Our Protection:</strong> Every downloaded list must match
              a known fingerprint. If even 1 character changes, validation
              fails immediately.
            </p>
          </div>

          <div style={{ marginBottom: "2.5rem" }}>
            <h3>🚨 2. Compromised List Providers</h3>
            <p style={{ fontSize: "1.05rem", lineHeight: "1.7" }}>
              <strong>The Threat:</strong> A popular filter list website gets
              hacked, and attackers replace legitimate lists with ones that
              block security warnings or allow phishing domains.
            </p>
            <p style={{ fontSize: "1.05rem", lineHeight: "1.7" }}>
              <strong>Our Protection:</strong> We store fingerprints of
              known-good versions. If a list changes unexpectedly, you're
              alerted and can review the changes before accepting them.
            </p>
          </div>

          <div style={{ marginBottom: "2.5rem" }}>
            <h3>🚨 3. Local File Tampering</h3>
            <p style={{ fontSize: "1.05rem", lineHeight: "1.7" }}>
              <strong>The Threat:</strong> Malware on your computer modifies
              your local filter lists to remove its own domains from blocklists
              or add legitimate security tools to the blocklist.
            </p>
            <p style={{ fontSize: "1.05rem", lineHeight: "1.7" }}>
              <strong>Our Protection:</strong> Before each compilation, we
              verify files haven't changed. If tampering is detected,
              compilation stops immediately.
            </p>
          </div>

          <div style={{ marginBottom: "2.5rem" }}>
            <h3>🚨 4. Typosquatting & Fake Lists</h3>
            <p style={{ fontSize: "1.05rem", lineHeight: "1.7" }}>
              <strong>The Threat:</strong> Attackers create fake lists with
              names similar to trusted lists (e.g., easy<strong>1</strong>ist.to
              instead of easy<strong>l</strong>ist.to).
            </p>
            <p style={{ fontSize: "1.05rem", lineHeight: "1.7" }}>
              <strong>Our Protection:</strong> We enforce HTTPS-only URLs,
              verify domains via DNS, and scan content to ensure it's actually
              a filter list.
            </p>
          </div>

          <div style={{ marginBottom: "2.5rem" }}>
            <h3>🚨 5. Supply Chain Attacks</h3>
            <p style={{ fontSize: "1.05rem", lineHeight: "1.7" }}>
              <strong>The Threat:</strong> Attackers compromise the build
              pipeline of a trusted filter list provider, injecting malicious
              rules into otherwise legitimate updates.
            </p>
            <p style={{ fontSize: "1.05rem", lineHeight: "1.7" }}>
              <strong>Our Protection:</strong> Cryptographic signatures and
              audit trails ensure that validation actually occurred and can't
              be forged.
            </p>
          </div>
        </div>
      </section>

      <section style={{ marginBottom: "3rem" }}>
        <h2>5 Layers of Protection</h2>

        <div className="features" style={{ marginTop: "2rem" }}>
          <div className="feature">
            <h3>🛡️ Layer 1: HTTPS-Only</h3>
            <p>
              Only allows secure downloads. HTTP connections can be intercepted
              and modified - we simply don't allow them.
            </p>
          </div>

          <div className="feature">
            <h3>🛡️ Layer 2: Domain Validation</h3>
            <p>
              Verifies domains are legitimate via DNS lookups, preventing
              typosquatting attacks and catching typos in URLs.
            </p>
          </div>

          <div className="feature">
            <h3>🛡️ Layer 3: Content Validation</h3>
            <p>
              Scans downloads to ensure they're actually filter lists, not
              malware or fake pages.
            </p>
          </div>

          <div className="feature">
            <h3>🛡️ Layer 4: Cryptographic Hashing</h3>
            <p>
              SHA-384 fingerprinting provides mathematical certainty. Tampering
              is impossible to hide.
            </p>
          </div>

          <div className="feature">
            <h3>🛡️ Layer 5: Audit Trail</h3>
            <p>
              Complete transparency with records of what was validated, when,
              and by which version.
            </p>
          </div>

          <div className="feature">
            <h3>🛡️ Bonus: Runtime Enforcement</h3>
            <p>
              Cryptographic proof that validation actually ran, preventing
              bypasses and ensuring compliance.
            </p>
          </div>
        </div>
      </section>

      <section style={{ marginBottom: "3rem" }}>
        <h2>Performance Impact</h2>
        <p style={{ fontSize: "1.1rem", lineHeight: "1.8" }}>
          Security shouldn't slow you down. Our validation system is designed
          for speed:
        </p>

        <table
          style={{
            width: "100%",
            marginTop: "1.5rem",
            borderCollapse: "collapse",
          }}
        >
          <thead>
            <tr style={{ backgroundColor: "#f8f9fa" }}>
              <th
                style={{
                  padding: "1rem",
                  textAlign: "left",
                  borderBottom: "2px solid #dee2e6",
                }}
              >
                Operation
              </th>
              <th
                style={{
                  padding: "1rem",
                  textAlign: "left",
                  borderBottom: "2px solid #dee2e6",
                }}
              >
                Time
              </th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <td style={{ padding: "1rem", borderBottom: "1px solid #dee2e6" }}>
                SHA-384 hash computation per file
              </td>
              <td style={{ padding: "1rem", borderBottom: "1px solid #dee2e6" }}>
                ~0.5ms
              </td>
            </tr>
            <tr>
              <td style={{ padding: "1rem", borderBottom: "1px solid #dee2e6" }}>
                URL validation (DNS lookup)
              </td>
              <td style={{ padding: "1rem", borderBottom: "1px solid #dee2e6" }}>
                ~10ms
              </td>
            </tr>
            <tr>
              <td style={{ padding: "1rem", borderBottom: "1px solid #dee2e6" }}>
                Syntax validation per file
              </td>
              <td style={{ padding: "1rem", borderBottom: "1px solid #dee2e6" }}>
                ~5ms
              </td>
            </tr>
            <tr style={{ backgroundColor: "#f8f9fa", fontWeight: "bold" }}>
              <td style={{ padding: "1rem" }}>Total overhead (typical)</td>
              <td style={{ padding: "1rem" }}>&lt;100ms</td>
            </tr>
          </tbody>
        </table>

        <p
          style={{
            fontSize: "1.05rem",
            marginTop: "1.5rem",
            fontStyle: "italic",
          }}
        >
          Compare this to downloading remote lists (500-5000ms) and compilation
          (1000-10000ms). The security overhead is negligible!
        </p>
      </section>

      <section style={{ marginBottom: "3rem" }}>
        <h2>Frequently Asked Questions</h2>

        <div style={{ marginTop: "2rem" }}>
          <details style={{ marginBottom: "1.5rem" }}>
            <summary
              style={{
                fontSize: "1.1rem",
                fontWeight: "bold",
                cursor: "pointer",
                padding: "0.5rem 0",
              }}
            >
              Can I disable validation if I trust my sources?
            </summary>
            <p style={{ marginTop: "1rem", marginLeft: "1.5rem" }}>
              <strong>No, and that's for your own safety.</strong> Validation
              is mandatory by design. Even trusted sources can be compromised.
              However, you can use "warning mode" for development which logs
              issues without failing compilation. For production, we strongly
              recommend "strict mode" which fails on any anomaly.
            </p>
          </details>

          <details style={{ marginBottom: "1.5rem" }}>
            <summary
              style={{
                fontSize: "1.1rem",
                fontWeight: "bold",
                cursor: "pointer",
                padding: "0.5rem 0",
              }}
            >
              What if a legitimate list updates and validation fails?
            </summary>
            <p style={{ marginTop: "1rem", marginLeft: "1.5rem" }}>
              <strong>This is a feature, not a bug!</strong> When validation
              detects a change, it alerts you to review what was modified. If
              the change is legitimate, you simply update your hash database and
              compilation proceeds. This gives you visibility and control over
              what enters your network.
            </p>
          </details>

          <details style={{ marginBottom: "1.5rem" }}>
            <summary
              style={{
                fontSize: "1.1rem",
                fontWeight: "bold",
                cursor: "pointer",
                padding: "0.5rem 0",
              }}
            >
              How do I know validation is actually running?
            </summary>
            <p style={{ marginTop: "1rem", marginLeft: "1.5rem" }}>
              Every compilation includes validation metadata with timestamps,
              file counts, and a cryptographic signature. Our CI/CD workflows
              fail if validation is bypassed, and you can use verification
              functions to programmatically confirm validation occurred.
            </p>
          </details>

          <details style={{ marginBottom: "1.5rem" }}>
            <summary
              style={{
                fontSize: "1.1rem",
                fontWeight: "bold",
                cursor: "pointer",
                padding: "0.5rem 0",
              }}
            >
              Why SHA-384 instead of SHA-256?
            </summary>
            <p style={{ marginTop: "1rem", marginLeft: "1.5rem" }}>
              SHA-384 provides enhanced security with ~2^192 operations required
              to find a collision (vs 2^128 for SHA-256), while still being fast
              enough for real-time validation (&lt;1ms per file). It's FIPS
              180-4 compliant and approved by NIST for cryptographic use.
            </p>
          </details>

          <details style={{ marginBottom: "1.5rem" }}>
            <summary
              style={{
                fontSize: "1.1rem",
                fontWeight: "bold",
                cursor: "pointer",
                padding: "0.5rem 0",
              }}
            >
              Is this overkill for personal use?
            </summary>
            <p style={{ marginTop: "1rem", marginLeft: "1.5rem" }}>
              <strong>No.</strong> Attacks don't discriminate by network size.
              The same techniques work against home users and enterprises alike.
              The best security is security you don't have to think about -
              validation handles the complexity automatically.
            </p>
          </details>
        </div>
      </section>

      <section
        style={{
          backgroundColor: "#d1ecf1",
          padding: "2rem",
          borderRadius: "8px",
          marginBottom: "3rem",
        }}
      >
        <h2>✅ The Bottom Line</h2>
        <ul
          style={{ fontSize: "1.1rem", lineHeight: "2", marginLeft: "1.5rem" }}
        >
          <li>
            <strong>Automatic:</strong> Works silently in the background - no
            configuration needed
          </li>
          <li>
            <strong>Fast:</strong> Adds less than 100ms to compilation time
          </li>
          <li>
            <strong>Comprehensive:</strong> Stops 5+ types of attacks
            automatically
          </li>
          <li>
            <strong>Proven:</strong> Uses NIST-approved SHA-384 cryptographic
            hashing
          </li>
          <li>
            <strong>Transparent:</strong> Complete audit trails and verification
          </li>
          <li>
            <strong>Universal:</strong> Protects everyone from home users to
            enterprises
          </li>
        </ul>
        <p style={{ fontSize: "1.1rem", marginTop: "1.5rem" }}>
          <strong>
            Remember: The best security is security you don't have to think
            about. Our validation system handles the complexity so you can focus
            on staying safe online.
          </strong>
        </p>
      </section>

      <section style={{ marginBottom: "3rem" }}>
        <h2>Learn More</h2>
        <div className="features">
          <div className="feature">
            <h3>
              <Link to="/WHY_VALIDATION_MATTERS">Why Validation Matters</Link>
            </h3>
            <p>
              Complete technical and non-technical guide to our validation
              system, including threat models and attack scenarios.
            </p>
          </div>
          <div className="feature">
            <h3>
              <Link to="/HASH_VERIFICATION">Hash Verification</Link>
            </h3>
            <p>
              Technical documentation on the hash verification callback system
              and implementation across all compilers.
            </p>
          </div>
          <div className="feature">
            <h3>
              <Link to="/RUNTIME_ENFORCEMENT">Runtime Enforcement</Link>
            </h3>
            <p>
              How cryptographic validation ensures security measures are
              actually executed and can't be bypassed.
            </p>
          </div>
          <div className="feature">
            <h3>
              <Link to="/VALIDATION_ENFORCEMENT">Validation Enforcement</Link>
            </h3>
            <p>
              CI/CD enforcement mechanisms and how we ensure validation is never
              skipped in production.
            </p>
          </div>
          <div className="feature">
            <h3>
              <Link to="/getting-started">Getting Started</Link>
            </h3>
            <p>
              Start using the toolkit with built-in security validation in under
              5 minutes.
            </p>
          </div>
          <div className="feature">
            <h3>
              <a href="https://github.com/jaypatrick/ad-blocking/blob/main/SECURITY.md">
                Security Policy
              </a>
            </h3>
            <p>
              Our commitment to security, vulnerability reporting, and
              responsible disclosure.
            </p>
          </div>
        </div>
      </section>
    </Layout>
  )
}

export default SecurityPage

export const Head = () => (
  <title>Security - AdGuard Tools and Utilities</title>
)
