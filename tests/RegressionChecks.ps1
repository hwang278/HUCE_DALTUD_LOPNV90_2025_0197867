$ErrorActionPreference = 'Stop'

$repoRoot = Split-Path -Parent $PSScriptRoot
$projectRoot = Join-Path $repoRoot 'RebarStair'
$viewModelPath = Join-Path $projectRoot 'RebarStair\ViewModel\RebarStairViewModel.cs'
$optionPath = Join-Path $projectRoot 'RebarStair\Model\RebarOption.cs'
$parameterSetterPath = Join-Path $projectRoot 'helpers\RebarParameterSetter.cs'
$projectPath = Join-Path $projectRoot 'RebarStair.csproj'

function Assert-True([bool]$condition, [string]$message) {
    if (-not $condition) {
        throw $message
    }
}

$viewModelFiles = @(
    $viewModelPath
    Join-Path $projectRoot 'RebarStair\ViewModel\PointSelection\RebarStairViewModel.PointSelection.cs'
    Join-Path $projectRoot 'RebarStair\ViewModel\RebarCreation\RebarStairViewModel.Geometry.cs'
    Join-Path $projectRoot 'RebarStair\ViewModel\RebarCreation\RebarStairViewModel.Type1.cs'
    Join-Path $projectRoot 'RebarStair\ViewModel\RebarCreation\RebarStairViewModel.Type3.cs'
    Join-Path $projectRoot 'RebarStair\ViewModel\RebarCreation\RebarStairViewModel.Landing.cs'
    Join-Path $projectRoot 'RebarStair\ViewModel\RebarCreation\RebarStairViewModel.Type2.cs'
)
$viewModel = ($viewModelFiles | Where-Object { Test-Path -LiteralPath $_ } | ForEach-Object { Get-Content -Raw -LiteralPath $_ }) -join "`n"
$option = Get-Content -Raw -LiteralPath $optionPath
$parameterSetter = Get-Content -Raw -LiteralPath $parameterSetterPath
$project = Get-Content -Raw -LiteralPath $projectPath

$imageMatches = [regex]::Matches($viewModel, '/RebarStair;component/Images/(?<name>[^"\s]+)')
$imageNames = $imageMatches | ForEach-Object { $_.Groups['name'].Value } | Sort-Object -Unique

foreach ($imageName in $imageNames) {
    $imagePath = Join-Path (Join-Path $projectRoot 'Images') $imageName
    Assert-True (Test-Path -LiteralPath $imagePath) "Missing image: $imageName"

    $resourcePattern = 'Resource Include="Images\\' + [regex]::Escape($imageName) + '"'
    Assert-True ([regex]::IsMatch($project, $resourcePattern)) "Image is not a WPF resource: $imageName"
}

Assert-True (-not $option.Contains('static event Action<RebarOptions>')) 'Rebar selection event must not be static.'
Assert-True (-not $parameterSetter.Contains('LookupParameter("Partition")')) 'Partition lookup must use its built-in parameter id.'

$expectedPlans = @(
    'Type1:0:rebar1,rebar2,rebar3,rebar45'
    'Type1:4:rebar1,rebar2'
    'Type3:0:rebar1,rebar2,rebar3,rebar4,rebar5'
    'Type3:5:rebar1,rebar2,rebar3,rebar4,rebar5,rebar6'
    'CN:0:rebar1,rebar2'
    'Type2:0:rebar1,rebar2,rebar3'
    'Type2:3:rebar1,rebar2'
)

$callMatches = [regex]::Matches(
    $viewModel,
    'RebarParameterSetter\.SetRebarParameters\((?<args>.*?)\);',
    [System.Text.RegularExpressions.RegexOptions]::Singleline)

$actualPlans = foreach ($callMatch in $callMatches) {
    $arguments = $callMatch.Groups['args'].Value -split ',' | ForEach-Object { $_.Trim() }
    $group = $arguments[1].Replace('RebarTypeGroup.', '')
    $startIndex = $arguments[2]
    $rebars = @($arguments[3..($arguments.Count - 1)])

    Assert-True (($rebars | Sort-Object -Unique).Count -eq $rebars.Count) "Duplicate rebar in $group partition batch."
    "$group`:$startIndex`:$($rebars -join ',')"
}

Assert-True (($actualPlans -join '|') -eq ($expectedPlans -join '|')) 'Partition assignment plan changed unexpectedly.'

Write-Output "Regression checks passed: $($imageNames.Count) images and $($actualPlans.Count) partition batches."
