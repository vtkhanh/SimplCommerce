Get-ChildItem "test" | where { ($_.PsIsContainer) -and ($_ -like "*Tests") } | %{
    pushd $_
    dotnet test
    popd
}
