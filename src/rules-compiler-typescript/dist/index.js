"use strict";
/**
 * Rules Compiler TypeScript Frontend
 *
 * A TypeScript API and CLI for compiling AdGuard filter rules
 * using @adguard/hostlist-compiler.
 *
 * @packageDocumentation
 */
Object.defineProperty(exports, "__esModule", { value: true });
exports.logger = exports.createLogger = exports.main = exports.getVersionInfo = exports.showVersion = exports.showHelp = exports.parseArgs = exports.runCompiler = exports.compileFilters = exports.copyToRulesDirectory = exports.computeHash = exports.countRules = exports.writeOutput = exports.toJson = exports.readConfiguration = exports.findDefaultConfig = exports.detectFormat = void 0;
// Configuration reader
var config_reader_1 = require("./config-reader");
Object.defineProperty(exports, "detectFormat", { enumerable: true, get: function () { return config_reader_1.detectFormat; } });
Object.defineProperty(exports, "findDefaultConfig", { enumerable: true, get: function () { return config_reader_1.findDefaultConfig; } });
Object.defineProperty(exports, "readConfiguration", { enumerable: true, get: function () { return config_reader_1.readConfiguration; } });
Object.defineProperty(exports, "toJson", { enumerable: true, get: function () { return config_reader_1.toJson; } });
// Compiler
var compiler_1 = require("./compiler");
Object.defineProperty(exports, "writeOutput", { enumerable: true, get: function () { return compiler_1.writeOutput; } });
Object.defineProperty(exports, "countRules", { enumerable: true, get: function () { return compiler_1.countRules; } });
Object.defineProperty(exports, "computeHash", { enumerable: true, get: function () { return compiler_1.computeHash; } });
Object.defineProperty(exports, "copyToRulesDirectory", { enumerable: true, get: function () { return compiler_1.copyToRulesDirectory; } });
Object.defineProperty(exports, "compileFilters", { enumerable: true, get: function () { return compiler_1.compileFilters; } });
Object.defineProperty(exports, "runCompiler", { enumerable: true, get: function () { return compiler_1.runCompiler; } });
// CLI
var cli_1 = require("./cli");
Object.defineProperty(exports, "parseArgs", { enumerable: true, get: function () { return cli_1.parseArgs; } });
Object.defineProperty(exports, "showHelp", { enumerable: true, get: function () { return cli_1.showHelp; } });
Object.defineProperty(exports, "showVersion", { enumerable: true, get: function () { return cli_1.showVersion; } });
Object.defineProperty(exports, "getVersionInfo", { enumerable: true, get: function () { return cli_1.getVersionInfo; } });
Object.defineProperty(exports, "main", { enumerable: true, get: function () { return cli_1.main; } });
// Logger
var logger_1 = require("./logger");
Object.defineProperty(exports, "createLogger", { enumerable: true, get: function () { return logger_1.createLogger; } });
Object.defineProperty(exports, "logger", { enumerable: true, get: function () { return logger_1.logger; } });
//# sourceMappingURL=index.js.map