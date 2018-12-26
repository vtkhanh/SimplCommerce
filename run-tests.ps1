Get-ChildItem "test" | where { ($_.PsIsContainer) -and ($_ -like "*Tests") } | %{
    pushd "test\$_"
    & dotnet test
    popd
}
