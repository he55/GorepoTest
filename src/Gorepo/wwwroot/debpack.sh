#!/bin/bash

TMPDIR=$(mktemp -d)

dpkg-scanpackages -m debs >$TMPDIR/Packages || exit 1
gzip -c9 $TMPDIR/Packages >$TMPDIR/Packages.gz
bzip2 -c9 $TMPDIR/Packages >$TMPDIR/Packages.bz2
xz -c9 $TMPDIR/Packages >$TMPDIR/Packages.xz

cp $TMPDIR/Packages* .
rm -r $TMPDIR
