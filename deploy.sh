#!/bin/bash -e

#TRAVIS_TAG=CommandDotNet.TestTools_1.0.0
#TRAVIS_TAG=CommandDotNet.TestTools_1.0.0-preview1

parseTravisTag () {
  echo " "
  echo ">>> parseTravisTag"
  echo " "

  if [[ $TRAVIS_TAG =~ (.+)_(.+) ]]
  then
    PROJECT_NAME=${BASH_REMATCH[1]}
    DEPLOYMENT_VERSION=${BASH_REMATCH[2]}
    echo "TRAVIS_TAG resolved to project $PROJECT_NAME & version $DEPLOYMENT_VERSION"
  else
    >&2 echo "failed to parse TRAVIS_TAG of value '$TRAVIS_TAG'"
    exit 1
  fi
  
  echo " "
  echo "<<< parseTravisTag"
  echo " "
}

parseProjectRefs () {  
  echo " "
  echo ">>> parseProjectRefs"
  echo " "

  ## get list of projects in solution referenced by PROJECT_NAME
  ## after grep, lines will look like ..\CommandDotNet\CommandDotNet.csproj
  ## sed removes start ..\
  while read -r projectRefFile; do    
    if [[ $projectRefFile =~ (.+)[/\\](.+) ]]
    then
      projectRefName=${BASH_REMATCH[1]}
      PROJECT_REF_NAMES+=($projectRefName)
    else
      >&2 echo "failed to parse projectRefFile of value '$projectRefFile'"
      exit 1
    fi

    tagDescr=`git describe --tags --abbrev=0 --match "$projectRefName"_*`

    if [[ $tagDescr =~ (.+)_(.+) ]]
    then
      projectRefVersion=${BASH_REMATCH[2]}
      PROJECT_REF_VERSIONS+=($projectRefVersion)
    else
      >&2 echo "failed to parse projectRefVersion of value '$tagDescr' for $projectRefName"
      exit 1
    fi
    
    #echo "projectRefFile    = $projectRefFile"
    #echo "projectRefName    = $projectRefName"
    #echo "projectRefVersion = $projectRefVersion"
    #echo "tagDescr          = $tagDescr"
    
  done  < <(dotnet list $PROJECT_FILE reference | grep csproj | sed 's/^...//')
  # ^^^ use process substitution instead of piping into while statement
  # keeps while loop in the same context so it can update the arrays
  # https://stackoverflow.com/questions/9985076/bash-populate-an-array-in-loop
  
  echo " "
  echo "<<< parseProjectRefs"
  echo " "
}

updateProjectRefsInSln() {
  echo " "
  echo " >>> updateProjectRefsInSln"
  echo " "

  for i in ${!PROJECT_REF_NAMES[@]}; do
    projectRefName=${PROJECT_REF_NAMES[$i]}
    projectRefVersion=${PROJECT_REF_VERSIONS[$i]}

    projectFile=$projectRefName/$projectRefName.csproj
    
    echo "update version in project $projectRefName $projectRefVersion"
    sed -i "s,<Version>1.0.0</Version>,<Version>$projectRefVersion</Version>," $projectFile    
  done
  
  echo " "
  echo " <<< updateProjectRefsInSln"
  echo " "
}

updateProjectRefsInNuspec () {
  echo " "
  echo " >>> updateProjectRefsInNuspec"
  echo " "
  
  for i in ${!PROJECT_REF_NAMES[@]}; do
    projectRefName=${PROJECT_REF_NAMES[$i]}
    projectRefVersion=${PROJECT_REF_VERSIONS[$i]}
   
    # dotnet pack has a bug: https://github.com/NuGet/Home/issues/7328
    # - project reference versions set to pack version 
    # so we'll check DEPLOYMENT_VERSION and also 1.0.0 so this still works when the bug is fixed     
    sed -i "s/id=\"$projectRefName\" version=\"1.0.0\"/id=\"$projectRefName\" version=\"$projectRefVersion\"/" $NUSPEC_FILE
    sed -i "s/id=\"$projectRefName\" version=\"$DEPLOYMENT_VERSION\"/id=\"$projectRefName\" version=\"$projectRefVersion\"/" $NUSPEC_FILE
  done
  
  echo " "
  echo " <<< updateProjectRefsInNuspec"
  echo " "
}

fixNupkgVersions () {
  echo " "
  echo " >>> fixNupkgVersions"
  echo " "
  
  # update nuspec with correct versions of referenced projects  
  cd output
  unzip $NUPKG_FILE $NUSPEC_FILE
  chmod 666 $NUSPEC_FILE
  
  updateProjectRefsInNuspec
  
  zip $NUPKG_FILE $NUSPEC_FILE
  rm $NUSPEC_FILE
  cd ..
  
  echo " "
  echo " <<< fixNupkgVersions"
  echo " "
}

parseTravisTag

PROJECT_FILE=$PROJECT_NAME/$PROJECT_NAME.csproj
NUPKG_FILE=$PROJECT_NAME.$DEPLOYMENT_VERSION.nupkg
NUSPEC_FILE=$PROJECT_NAME.nuspec

declare -a PROJECT_REF_NAMES
declare -a PROJECT_REF_VERSIONS

echo "PROJECT_NAME         = $PROJECT_NAME"
echo "DEPLOYMENT_VERSION   = $DEPLOYMENT_VERSION"
echo "PROJECT_FILE         = $PROJECT_FILE"
echo "NUPKG_FILE           = $NUPKG_FILE"
echo "NUSPEC_FILE          = $NUSPEC_FILE"

if [ "$PROJECT_NAME" != "CommandDotNet" ]
then
  parseProjectRefs
fi
  
echo "PROJECT_REF_NAMES    = ${PROJECT_REF_NAMES[@]}"
echo "PROJECT_REF_VERSIONS = ${PROJECT_REF_VERSIONS[@]}"
  
updateProjectRefsInSln

echo " "
echo ">>> pack"

# PACKAGE
dotnet pack \
-o output \
-c Release \
$PROJECT_FILE \
-p:Version=$DEPLOYMENT_VERSION \
#--no-restore \
#-v:diag

echo "<<< pack"
echo " "

fixNupkgVersions

# PUBLISH TO NUGET
dotnet nuget push -s https://api.nuget.org/v3/index.json -k $NUGET_API_KEY_COMMANDDOTNET output/$NUPKG_FILE