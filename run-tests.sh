#!/usr/bin/env bash

echo ""
echo "Run tests..."
echo ""

for test in ./test/*/
do
    echo "Testing $test"
    pushd "$test"
    dotnet test -c Release --no-restore --no-build
    popd
done