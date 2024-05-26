#/usr/bin/env bash
set -e

reporoot=$(git rev-parse --show-toplevel)
pkgdir="$reporoot/package/debian/pkg"

rm -rf "$pkgdir"

install -Dm755 "$reporoot/target/release/blent" "$pkgdir/usr/bin/blent"

install -Dm644 "$reporoot/package/debian/control" "$pkgdir/DEBIAN/control"
(cd $pkgdir/.. && dpkg-deb --build --root-owner-group pkg)
