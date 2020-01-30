#/bin/bash

version=$1

if [[ $1 =~ ^[0-9].[0-9].[0-9]$ ]]; then
    echo "creating release date: " $1
else
    echo "invalid relsase version:" $1
    exit 1
fi

git subtree split --prefix=CodeDocs.UnityPackage/Assets/Gamesture.CodeDocs --branch upm
git tag $1 upm