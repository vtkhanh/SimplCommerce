#!/usr/bin/env bash

# TODO: 
# Temporarily solution to fix fucking hanging build!!!
# Don't use Bundle&Minify feature

echo ""
echo "Rename bundleconfig..."
echo ""

for f in ./**/bundleconfig.json
do 
    echo $f;
    mv "$f" $(echo "$f" | sed 's/bundle/_bundle/g');
done

