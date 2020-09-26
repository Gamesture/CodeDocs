#/bin/bash

function check_last_error_code
{
    error_code="${?}"
    if [ $error_code -ne 0 ]; then
        echo " ERROR! (code: ${error_code})"
        exit $error_code
    fi
}

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
git add ./CodeDocs.UnityPackage/Assets/Gamesture.CodeDocs/package.json
check_last_error_code
git commit -m "version $1"
check_last_error_code
git push
check_last_error_code
cd ./CodeDocs.UnityPackage/Assets/Gamesture.CodeDocs && npm publish --registry http://34.107.237:4873 && cd -
check_last_error_code