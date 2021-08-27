using System;
using System.Collections.Generic;
using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Representation.Functions;

namespace Symbolica.Representation
{
    public static class DeclarationFactory
    {
        private static readonly IReadOnlyDictionary<string, Func<FunctionId, IParameters, IFunction>> Intrinsics =
            new Dictionary<string, Func<FunctionId, IParameters, IFunction>>
            {
                {"llvm.abs", (id, parameters) => new Absolute(id, parameters)},
                {"llvm.ceil", (id, parameters) => new Ceiling(id, parameters)},
                {"llvm.ctpop", (id, parameters) => new CountOnes(id, parameters)},
                {"llvm.experimental.constrained.ceil", (id, parameters) => new Ceiling(id, parameters)},
                {"llvm.experimental.constrained.fadd", (id, parameters) => new FloatAdd(id, parameters)},
                {"llvm.experimental.constrained.fdiv", (id, parameters) => new FloatDivide(id, parameters)},
                {"llvm.experimental.constrained.floor", (id, parameters) => new Floor(id, parameters)},
                {"llvm.experimental.constrained.fmul", (id, parameters) => new FloatMultiply(id, parameters)},
                {"llvm.experimental.constrained.fptosi", (id, parameters) => new FloatToSigned(id, parameters)},
                {"llvm.experimental.constrained.fptoui", (id, parameters) => new FloatToUnsigned(id, parameters)},
                {"llvm.experimental.constrained.frem", (id, parameters) => new FloatRemainder(id, parameters)},
                {"llvm.experimental.constrained.fsub", (id, parameters) => new FloatSubtract(id, parameters)},
                {"llvm.experimental.constrained.sitofp", (id, parameters) => new SignedToFloat(id, parameters)},
                {"llvm.experimental.constrained.uitofp", (id, parameters) => new UnsignedToFloat(id, parameters)},
                {"llvm.floor", (id, parameters) => new Floor(id, parameters)},
                {"llvm.fshl", (id, parameters) => new FunnelShiftLeft(id, parameters)},
                {"llvm.fshr", (id, parameters) => new FunnelShiftRight(id, parameters)},
                {"llvm.memcpy", (id, parameters) => new MemoryCopy(id, parameters)},
                {"llvm.memmove", (id, parameters) => new MemoryMove(id, parameters)},
                {"llvm.memset", (id, parameters) => new MemorySet(id, parameters)},
                {"llvm.stackrestore", (id, parameters) => new StackRestore(id, parameters)},
                {"llvm.stacksave", (id, parameters) => new StackSave(id, parameters)},
                {"llvm.va_copy", (id, parameters) => new VariadicCopy(id, parameters)},
                {"llvm.va_end", (id, parameters) => new VariadicEnd(id, parameters)},
                {"llvm.va_start", (id, parameters) => new VariadicStart(id, parameters)}
            };

        private static readonly IReadOnlyDictionary<string, Func<FunctionId, IParameters, IFunction>> Specials =
            new Dictionary<string, Func<FunctionId, IParameters, IFunction>>
            {
                {"__assert_fail", (id, parameters) => new Fail(id, parameters)},
                {"calloc", (id, parameters) => new AllocateAndClear(id, parameters)},
                {"ceil", (id, parameters) => new Ceiling(id, parameters)},
                {"__clock_gettime", (id, parameters) => new GetTime(id, parameters)},
                {"clock_gettime", (id, parameters) => new GetTime(id, parameters)},
                {"close", (id, parameters) => new Close(id, parameters)},
                {"dup", (id, parameters) => new Duplicate(id, parameters)},
                {"_exit", (id, parameters) => new Exit(id, parameters)},
                {"fcntl", (id, parameters) => new FileControl(id, parameters)},
                {"floor", (id, parameters) => new Floor(id, parameters)},
                {"free", (id, parameters) => new Free(id, parameters)},
                {"fstat", (id, parameters) => new GetStatus(id, parameters)},
                {"fstat64", (id, parameters) => new GetStatus(id, parameters)},
                {"GC_clear_roots", (id, parameters) => new NoOp(id, parameters)},
                {"GC_disable", (id, parameters) => new NoOp(id, parameters)},
                {"GC_enable", (id, parameters) => new NoOp(id, parameters)},
                {"GC_free", (id, parameters) => new Free(id, parameters)},
                {"GC_get_stack_base", (id, parameters) => new GcUnimplemented(id, parameters)},
                {"GC_get_start_callback", (id, parameters) => new Null(id, parameters)},
                {"GC_init", (id, parameters) => new NoOp(id, parameters)},
                {"GC_malloc", (id, parameters) => new Allocate(id, parameters)},
                {"GC_malloc_atomic", (id, parameters) => new Allocate(id, parameters)},
                {"GC_malloc_atomic_ignore_off_page", (id, parameters) => new Allocate(id, parameters)},
                {"GC_malloc_ignore_off_page", (id, parameters) => new Allocate(id, parameters)},
                {"GC_malloc_uncollectable", (id, parameters) => new Allocate(id, parameters)},
                {"GC_register_displacement", (id, parameters) => new NoOp(id, parameters)},
                {"GC_register_finalizer_no_order", (id, parameters) => new NoOp(id, parameters)},
                {"GC_set_all_interior_pointers", (id, parameters) => new NoOp(id, parameters)},
                {"GC_set_java_finalization", (id, parameters) => new NoOp(id, parameters)},
                {"GC_set_max_heap_size", (id, parameters) => new NoOp(id, parameters)},
                {"GC_set_no_dls", (id, parameters) => new NoOp(id, parameters)},
                {"GC_set_oom_fn", (id, parameters) => new NoOp(id, parameters)},
                {"GC_set_start_callback", (id, parameters) => new NoOp(id, parameters)},
                {"GC_set_time_limit", (id, parameters) => new NoOp(id, parameters)},
                {"GC_set_warn_proc", (id, parameters) => new NoOp(id, parameters)},
                {"getcwd", (id, parameters) => new GetWorkingDirectory(id, parameters)},
                {"getrandom", (id, parameters) => new GetRandom(id, parameters)},
                {"getrlimit", (id, parameters) => new GetResourceLimit(id, parameters)},
                {"getrlimit64", (id, parameters) => new GetResourceLimit(id, parameters)},
                {"isatty", (id, parameters) => new IsTerminal(id, parameters)},
                {"__libc_malloc_impl", (id, parameters) => new Allocate(id, parameters)},
                {"__localtime_r", (id, parameters) => new GetLocalTime(id, parameters)},
                {"localtime_r", (id, parameters) => new GetLocalTime(id, parameters)},
                {"_longjmp", (id, parameters) => new LongJump(id, parameters)},
                {"longjmp", (id, parameters) => new LongJump(id, parameters)},
                {"__lseek", (id, parameters) => new Seek(id, parameters)},
                {"lseek", (id, parameters) => new Seek(id, parameters)},
                {"lseek64", (id, parameters) => new Seek(id, parameters)},
                {"lstat", (id, parameters) => new GetPathStatus(id, parameters)},
                {"lstat64", (id, parameters) => new GetPathStatus(id, parameters)},
                {"malloc", (id, parameters) => new Allocate(id, parameters)},
                {"__mmap", (id, parameters) => new MemoryMap(id, parameters)},
                {"mmap", (id, parameters) => new MemoryMap(id, parameters)},
                {"mmap64", (id, parameters) => new MemoryMap(id, parameters)},
                {"open", (id, parameters) => new Open(id, parameters)},
                {"open64", (id, parameters) => new Open(id, parameters)},
                {"__pthread_testcancel", (id, parameters) => new NoOp(id, parameters)},
                {"pthread_testcancel", (id, parameters) => new NoOp(id, parameters)},
                {"read", (id, parameters) => new Read(id, parameters)},
                {"readdir", (id, parameters) => new ReadDirectory(id, parameters)},
                {"readdir64", (id, parameters) => new ReadDirectory(id, parameters)},
                {"realloc", (id, parameters) => new Reallocate(id, parameters)},
                {"_setjmp", (id, parameters) => new SetupJump(id, parameters)},
                {"setjmp", (id, parameters) => new SetupJump(id, parameters)},
                {"sigaction", (id, parameters) => new SignalAction(id, parameters)},
                {"sigprocmask", (id, parameters) => new SignalMask(id, parameters)},
                {"sigaltstack", (id, parameters) => new SignalStack(id, parameters)},
                {"stat", (id, parameters) => new GetPathStatus(id, parameters)},
                {"stat64", (id, parameters) => new GetPathStatus(id, parameters)},
                {"symbolize", (id, parameters) => new Symbolize(id, parameters)},
                {"__vdsosym", (id, parameters) => new VirtualCall(id, parameters)}
            };

        private static readonly IReadOnlyDictionary<string, Func<FunctionId, IParameters, IFunction>> Internals =
            new Dictionary<string, Func<FunctionId, IParameters, IFunction>>
            {
                {"__get_tp", (id, parameters) => new GetThreadAddress(id, parameters)}
            };

        public static string Pattern => string.Join('|', Specials.Keys
            .Concat(Internals.Keys)
            .Concat(Internals.Keys.Select(s => $@"{s}\.[0-9]+"))
            .Select(s => $"^{s}$"));

        public static IFunction Create(string name, FunctionId id, IParameters parameters)
        {
            var func = Specials.TryGetValue(name, out var constructor)
                ? (_, i, p) => constructor(i, p)
                : CreateIntrinsicsAndInternals(name);

            return func(name, id, parameters);
        }

        private static Func<string, FunctionId, IParameters, IFunction> CreateIntrinsicsAndInternals(string name)
        {
            return Intrinsics.TryGetValue(name, out var constructor) || Internals.TryGetValue(name, out constructor)
                ? (_, i, p) => constructor(i, p)
                : CreateOverloaded(name);
        }

        private static Func<string, FunctionId, IParameters, IFunction> CreateOverloaded(string name)
        {
            var index = name.LastIndexOf('.');

            return index == -1
                ? (n, i, p) => new Unsupported(n, i, p)
                : CreateIntrinsicsAndInternals(name.Remove(index));
        }
    }
}
