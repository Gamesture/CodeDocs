#/bin/bash

version=$1

if [[ $1 =~ ^[0-9]+\.[0-9]+\.[0-9]+$ ]]; then
    echo "creating release date: " $1
else
    echo "invalid relsase version:" $1
    exit 1
fi

script_dir="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
cd ${script_dir}

find ./CodeDocs.UnityPackage/Assets/Gamesture.CodeDocs -name package.json -exec sed -i '' 's/"version": "[0-9]*\.[0-9]*\.[0-9]*"/"version": "'$1"\""/g {} \;
git add -A
git commit -m "version $1"
git push