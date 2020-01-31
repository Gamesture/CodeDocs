#!/bin/bash

PATH=$PATH:/usr/local/bin

command -v brew >/dev/null 2>&1 || { echo >&2 "I require brew but it's not installed.  Aborting."; exit 1; }
command -v doxygen >/dev/null 2>&1 || { echo >&2 "I require doxygen but it's not installed (use brew to install it).  Aborting."; exit 1; }

script_dir="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
cd ${script_dir}
sed s~_gamesture_product_~${1}~g Doxyfile > Doxyfile_product~
sed s~_gamesture_sources_~${2}~g Doxyfile_product~ > Doxyfile_mod~
rm Doxyfile_product~
rm -rf GeneratedDocs~
doxygen Doxyfile_mod~
rm Doxyfile_mod~