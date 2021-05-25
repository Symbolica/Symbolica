#!/bin/bash
set -e

outDir=./.symbolica
mkdir -p $outDir

extractFile=$(mktemp) || exit 1
trap "rm -f $extractFile" 0 2 3 15

optimizedFile=$(mktemp) || exit 1
trap "rm -f $optimizedFile" 0 2 3 15

extractArgs="^__assert_fail$|^calloc$|^ceil$|^__clock_gettime$|^clock_gettime$|^close$|^dup$|^_exit$|^fcntl$|^floor$|^free$|^fstat$|^fstat64$|^GC_clear_roots$|^GC_disable$|^GC_enable$|^GC_free$|^GC_get_stack_base$|^GC_get_start_callback$|^GC_init$|^GC_malloc$|^GC_malloc_atomic$|^GC_malloc_atomic_ignore_off_page$|^GC_malloc_ignore_off_page$|^GC_malloc_uncollectable$|^GC_register_displacement$|^GC_register_finalizer_no_order$|^GC_set_all_interior_pointers$|^GC_set_java_finalization$|^GC_set_max_heap_size$|^GC_set_no_dls$|^GC_set_oom_fn$|^GC_set_start_callback$|^GC_set_time_limit$|^GC_set_warn_proc$|^getcwd$|^getrandom$|^getrlimit$|^getrlimit64$|^isatty$|^__libc_malloc_impl$|^__localtime_r$|^localtime_r$|^_longjmp$|^longjmp$|^__lseek$|^lseek$|^lseek64$|^lstat$|^lstat64$|^malloc$|^__mmap$|^mmap$|^mmap64$|^open$|^open64$|^__pthread_testcancel$|^pthread_testcancel$|^read$|^readdir$|^readdir64$|^realloc$|^_setjmp$|^setjmp$|^sigaction$|^sigprocmask$|^sigaltstack$|^stat$|^stat64$|^symbolize$|^__vdsosym$|^__get_tp$|^__get_tp\.[0-9]+$"

llvm-extract -delete -ralias "$extractArgs" -rfunc "$extractArgs" "$1" -o $extractFile
opt -disable-loop-vectorization -strip-debug -loweratomic -Oz -scalarizer $extractFile -o $optimizedFile

dis $optimizedFile "$outDir/dis.json"
