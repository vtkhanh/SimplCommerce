#!/usr/bin/env bash
set -ev

for test in ./test/*Tests/
do
    echo "Testing $test"
    pushd "$test"
    dotnet test -c Release --no-restore
    popd
done
