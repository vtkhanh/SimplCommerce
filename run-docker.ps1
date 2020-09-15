param (
    [switch] $production,
    [switch] $nobuild
)

if ($nobuild -eq $false)
{
    $env = if ($production) { "Production" } else { "Docker" }

    Copy-Item "src\SimplCommerce.WebHost\appsettings.$env.json" "src\SimplCommerce.WebHost\appsettings.json"

    docker build -t kk-image:latest .
}

docker run --rm -it -p 8080:5000 kk-image:latest
