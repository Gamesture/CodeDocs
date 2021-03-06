#!/bin/bash

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
    echo "creating release: " $1
else
    echo "invalid release version:" $1
    exit 1
fi

script_dir="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
cd ${script_dir}

package_file=./CodeDocs.UnityPackage/Assets/Gamesture.CodeDocs/package.json
sed -i 's/"version": "[0-9]*\.[0-9]*\.[0-9]*"/"version": "'$1"\""/g ${package_file}
git add ${package_file}
check_last_error_code
git commit -m "version $1"
check_last_error_code
git push
check_last_error_code
cd ./CodeDocs.UnityPackage/Assets/Gamesture.CodeDocs && npm publish --registry https://unity-npm.admin-gamesture.com/ && cd -
check_last_error_code
