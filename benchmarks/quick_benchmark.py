#!/usr/bin/env python3
"""
Quick Synthetic Benchmark for Chunking Performance

This script simulates the chunking process to show expected speedups
without requiring the actual hostlist-compiler or full setup.

It demonstrates:
1. How rules are split into chunks
2. Simulated parallel processing time
3. Expected speedup ratios

Usage:
    python quick_benchmark.py
    python quick_benchmark.py --rules 500000 --parallel 8
    python quick_benchmark.py --interactive
"""

import argparse
import os
import random
import string
import time
from concurrent.futures import ThreadPoolExecutor, as_completed
from dataclasses import dataclass
from typing import List


@dataclass
class SimulatedChunk:
    """A simulated chunk of rules."""
    index: int
    total: int
    rule_count: int
    processing_time_ms: float = 0


@dataclass
class BenchmarkResult:
    """Result of a simulated benchmark."""
    total_rules: int
    chunk_count: int
    max_parallel: int
    sequential_time_ms: float
    parallel_time_ms: float
    speedup: float
    efficiency: float  # speedup / max_parallel


def generate_synthetic_rules(count: int) -> List[str]:
    """Generate synthetic filter rules (fast, in-memory)."""
    rules = []
    tlds = ["com", "net", "org", "io", "co"]
    words = ["ad", "ads", "track", "pixel", "banner", "promo", "click"]

    for _ in range(count):
        word = random.choice(words)
        suffix = ''.join(random.choices(string.ascii_lowercase, k=4))
        tld = random.choice(tlds)
        rules.append(f"||{word}{suffix}.{tld}^")

    return rules


def simulate_chunk_processing(chunk: SimulatedChunk, base_time_per_1k: float = 10) -> SimulatedChunk:
    """
    Simulate processing a chunk of rules.

    The base_time_per_1k parameter represents the approximate time in ms
    to process 1000 rules. This simulates real-world compilation overhead.
    """
    # Simulate processing time based on rule count
    # Real compilation has some fixed overhead plus per-rule time
    fixed_overhead_ms = 50  # Startup cost
    per_rule_ms = base_time_per_1k / 1000

    processing_time = fixed_overhead_ms + (chunk.rule_count * per_rule_ms)

    # Add some random variation (5-15%)
    variation = random.uniform(0.95, 1.15)
    processing_time *= variation

    # Actually sleep to simulate the work (scaled down for quick demo)
    # In real use, this would be actual compilation
    time.sleep(processing_time / 1000 * 0.01)  # 1% of actual time for demo

    chunk.processing_time_ms = processing_time
    return chunk


def split_into_chunks(rules: List[str], max_parallel: int) -> List[SimulatedChunk]:
    """Split rules into chunks for parallel processing."""
    if not rules:
        return []

    chunk_size = (len(rules) + max_parallel - 1) // max_parallel
    chunks = []

    for i in range(max_parallel):
        start = i * chunk_size
        end = min(start + chunk_size, len(rules))
        if start < len(rules):
            chunks.append(SimulatedChunk(
                index=i,
                total=max_parallel,
                rule_count=end - start,
            ))

    return chunks


def run_sequential_benchmark(rules: List[str], base_time_per_1k: float = 10) -> float:
    """Run a sequential (non-chunked) benchmark."""
    chunk = SimulatedChunk(index=0, total=1, rule_count=len(rules))
    result = simulate_chunk_processing(chunk, base_time_per_1k)
    return result.processing_time_ms


def run_parallel_benchmark(
    rules: List[str],
    max_parallel: int,
    base_time_per_1k: float = 10
) -> tuple[float, List[SimulatedChunk]]:
    """Run a parallel (chunked) benchmark."""
    chunks = split_into_chunks(rules, max_parallel)

    if not chunks:
        return 0, []

    start_time = time.perf_counter()

    # Process chunks in parallel
    with ThreadPoolExecutor(max_workers=max_parallel) as executor:
        futures = [
            executor.submit(simulate_chunk_processing, chunk, base_time_per_1k)
            for chunk in chunks
        ]

        completed_chunks = []
        for future in as_completed(futures):
            completed_chunks.append(future.result())

    # The parallel time is the wall-clock time, not sum of individual times
    parallel_time_ms = (time.perf_counter() - start_time) * 1000

    # But for theoretical comparison, we use the max individual chunk time
    # since that's the limiting factor in parallel execution
    max_chunk_time = max(c.processing_time_ms for c in completed_chunks)

    return max_chunk_time, completed_chunks


def run_benchmark(
    rule_count: int,
    max_parallel: int,
    base_time_per_1k: float = 10,
    verbose: bool = True
) -> BenchmarkResult:
    """Run a complete benchmark comparing sequential vs parallel."""
    if verbose:
        print(f"\nGenerating {rule_count:,} synthetic rules...")

    rules = generate_synthetic_rules(rule_count)

    if verbose:
        print(f"Running sequential benchmark...")

    sequential_time = run_sequential_benchmark(rules, base_time_per_1k)

    if verbose:
        print(f"Running parallel benchmark ({max_parallel} workers)...")

    parallel_time, chunks = run_parallel_benchmark(rules, max_parallel, base_time_per_1k)

    speedup = sequential_time / parallel_time if parallel_time > 0 else 1.0
    efficiency = speedup / max_parallel

    return BenchmarkResult(
        total_rules=rule_count,
        chunk_count=len(chunks),
        max_parallel=max_parallel,
        sequential_time_ms=sequential_time,
        parallel_time_ms=parallel_time,
        speedup=speedup,
        efficiency=efficiency,
    )


def print_result(result: BenchmarkResult) -> None:
    """Print a benchmark result."""
    print("\n" + "=" * 60)
    print("BENCHMARK RESULT")
    print("=" * 60)
    print(f"Total rules:         {result.total_rules:,}")
    print(f"Chunks:              {result.chunk_count}")
    print(f"Max parallel:        {result.max_parallel}")
    print("-" * 40)
    print(f"Sequential time:     {result.sequential_time_ms:,.0f} ms")
    print(f"Parallel time:       {result.parallel_time_ms:,.0f} ms")
    print(f"Speedup:             {result.speedup:.2f}x")
    print(f"Efficiency:          {result.efficiency:.1%}")
    print("-" * 40)
    print(f"Time saved:          {result.sequential_time_ms - result.parallel_time_ms:,.0f} ms")
    print("=" * 60)


def run_comparison_suite(max_parallel: int = None) -> None:
    """Run a suite of benchmarks comparing different rule counts."""
    if max_parallel is None:
        max_parallel = min(os.cpu_count() or 4, 8)

    print("\n" + "=" * 70)
    print("CHUNKING PERFORMANCE COMPARISON SUITE")
    print("=" * 70)
    print(f"CPU cores available: {os.cpu_count()}")
    print(f"Max parallel workers: {max_parallel}")
    print()
    print("This benchmark simulates compilation of filter lists of varying sizes")
    print("to show the expected speedup from parallel chunked compilation.")
    print()

    # Test different rule counts
    test_sizes = [
        (10_000, "Small (10K) - Typical personal list"),
        (50_000, "Medium (50K) - Standard blocklist"),
        (200_000, "Large (200K) - Combined list"),
        (500_000, "XLarge (500K) - Mega blocklist"),
    ]

    results = []

    print(f"{'Size':<15} {'Sequential':<15} {'Parallel':<15} {'Speedup':<12} {'Efficiency'}")
    print("-" * 70)

    for rule_count, description in test_sizes:
        result = run_benchmark(rule_count, max_parallel, verbose=False)
        results.append((description, result))

        size_str = f"{rule_count // 1000}K rules"
        seq_str = f"{result.sequential_time_ms:,.0f} ms"
        par_str = f"{result.parallel_time_ms:,.0f} ms"
        speedup_str = f"{result.speedup:.2f}x"
        eff_str = f"{result.efficiency:.0%}"

        print(f"{size_str:<15} {seq_str:<15} {par_str:<15} {speedup_str:<12} {eff_str}")

    print("-" * 70)

    # Summary
    avg_speedup = sum(r.speedup for _, r in results) / len(results)
    max_speedup = max(r.speedup for _, r in results)

    print(f"\nAverage speedup: {avg_speedup:.2f}x")
    print(f"Maximum speedup: {max_speedup:.2f}x")
    print()
    print("Note: Actual speedup depends on:")
    print("  - Number of CPU cores")
    print("  - I/O performance (especially for network sources)")
    print("  - Rule complexity and transformations applied")
    print("  - Memory bandwidth")


def run_parallel_scaling_test() -> None:
    """Test how speedup scales with number of parallel workers."""
    print("\n" + "=" * 70)
    print("PARALLEL SCALING TEST")
    print("=" * 70)
    print("Testing how speedup scales with different numbers of parallel workers")
    print("Using 200,000 rules as baseline")
    print()

    rule_count = 200_000
    cpu_count = os.cpu_count() or 4

    print(f"{'Workers':<10} {'Time (ms)':<15} {'Speedup':<12} {'Efficiency'}")
    print("-" * 50)

    # Get baseline (sequential)
    baseline = run_benchmark(rule_count, 1, verbose=False)
    print(f"{'1':<10} {baseline.sequential_time_ms:,.0f} ms{'':<6} {'1.00x':<12} {'100%'}")

    # Test different parallelism levels
    for workers in [2, 4, 6, 8, 12, 16]:
        if workers > cpu_count:
            continue

        result = run_benchmark(rule_count, workers, verbose=False)
        speedup = baseline.sequential_time_ms / result.parallel_time_ms
        efficiency = speedup / workers

        print(f"{workers:<10} {result.parallel_time_ms:,.0f} ms{'':<6} {speedup:.2f}x{'':<8} {efficiency:.0%}")

    print("-" * 50)
    print(f"\nYour system has {cpu_count} CPU cores.")
    print(f"Recommended max_parallel: {min(cpu_count, 8)}")


def interactive_mode() -> None:
    """Run in interactive mode with user prompts."""
    print("\n" + "=" * 70)
    print("INTERACTIVE CHUNKING BENCHMARK")
    print("=" * 70)
    print()

    while True:
        print("\nOptions:")
        print("  1. Run comparison suite (all sizes)")
        print("  2. Run parallel scaling test")
        print("  3. Custom benchmark")
        print("  4. Exit")
        print()

        choice = input("Enter choice (1-4): ").strip()

        if choice == "1":
            run_comparison_suite()
        elif choice == "2":
            run_parallel_scaling_test()
        elif choice == "3":
            try:
                rules = int(input("Enter number of rules (e.g., 100000): "))
                workers = int(input(f"Enter max parallel workers (1-{os.cpu_count()}): "))
                result = run_benchmark(rules, workers)
                print_result(result)
            except ValueError:
                print("Invalid input, please enter numbers")
        elif choice == "4":
            print("Goodbye!")
            break
        else:
            print("Invalid choice")


def main():
    parser = argparse.ArgumentParser(
        description="Quick synthetic benchmark for chunking performance"
    )
    parser.add_argument(
        "--rules",
        type=int,
        default=200_000,
        help="Number of rules to simulate (default: 200000)"
    )
    parser.add_argument(
        "--parallel",
        type=int,
        default=None,
        help=f"Max parallel workers (default: CPU count, max 8)"
    )
    parser.add_argument(
        "--suite",
        action="store_true",
        help="Run the full comparison suite"
    )
    parser.add_argument(
        "--scaling",
        action="store_true",
        help="Run the parallel scaling test"
    )
    parser.add_argument(
        "--interactive",
        action="store_true",
        help="Run in interactive mode"
    )

    args = parser.parse_args()

    if args.interactive:
        interactive_mode()
    elif args.suite:
        run_comparison_suite(args.parallel)
    elif args.scaling:
        run_parallel_scaling_test()
    else:
        max_parallel = args.parallel or min(os.cpu_count() or 4, 8)
        result = run_benchmark(args.rules, max_parallel)
        print_result(result)


if __name__ == "__main__":
    main()
