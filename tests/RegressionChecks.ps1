$ErrorActionPreference = 'Stop'

$repoRoot = Split-Path -Parent $PSScriptRoot
$projectRoot = Join-Path $repoRoot 'RebarStair'
$viewModelPath = Join-Path $projectRoot 'RebarStair\ViewModel\RebarStairViewModel.cs'
$optionPath = Join-Path $projectRoot 'RebarStair\Model\RebarOption.cs'
$parameterSetterPath = Join-Path $projectRoot 'helpers\RebarParameterSetter.cs'
$projectPath = Join-Path $projectRoot 'RebarStair.csproj'
$manifestPath = Join-Path $projectRoot 'RebarStairAddin.addin'
$viewPath = Join-Path $projectRoot 'RebarStair\View\RebarStairView.xaml'
$commandPath = Join-Path $projectRoot 'RebarStair\RebarStairCmd.cs'
$previewSessionPath = Join-Path $projectRoot 'helpers\RevitPreviewSession.cs'
$previewPath = Join-Path $projectRoot 'RebarStair\ViewModel\RebarStairViewModel.Preview.cs'
$geometryInspectorPath = Join-Path $projectRoot 'helpers\GeometryInspector.cs'
$selectionFilterPath = Join-Path $projectRoot 'Filter\StairSelectionFilter.cs'

function Assert-True([bool]$condition, [string]$message) {
    if (-not $condition) {
        throw $message
    }
}

$viewModelFiles = @(
    $viewModelPath
    Join-Path $projectRoot 'RebarStair\Services\PointSelectionService.cs'
    Join-Path $projectRoot 'RebarStair\Services\SectionService.cs'
    Join-Path $projectRoot 'RebarStair\ViewModel\RebarStairViewModel.Preview.cs'
    Join-Path $projectRoot 'RebarStair\Services\RebarPlacementService.cs'
    Join-Path $projectRoot 'RebarStair\Services\RebarPlacementService.Type1.cs'
    Join-Path $projectRoot 'RebarStair\Services\RebarPlacementService.Type3.cs'
    Join-Path $projectRoot 'RebarStair\Services\RebarPlacementService.Landing.cs'
    Join-Path $projectRoot 'RebarStair\Services\RebarPlacementService.Type2.cs'
)
$viewModel = ($viewModelFiles | Where-Object { Test-Path -LiteralPath $_ } | ForEach-Object { Get-Content -Raw -LiteralPath $_ }) -join "`n"
$option = Get-Content -Raw -LiteralPath $optionPath
$parameterSetter = Get-Content -Raw -LiteralPath $parameterSetterPath
$project = Get-Content -Raw -LiteralPath $projectPath
$manifest = Get-Content -Raw -LiteralPath $manifestPath
$view = Get-Content -Raw -LiteralPath $viewPath
$command = Get-Content -Raw -LiteralPath $commandPath
$previewSession = Get-Content -Raw -LiteralPath $previewSessionPath
$geometryInspector = Get-Content -Raw -LiteralPath $geometryInspectorPath
$selectionFilter = Get-Content -Raw -LiteralPath $selectionFilterPath
$takeoff = Get-Content -Raw -LiteralPath $previewPath

Assert-True (-not $project.Contains('Su.Revit.HelixToolkit.SharpDX')) 'Native Revit preview must not ship Helix/SharpDX.'
Assert-True ($project.Contains('<TargetFramework>net10.0-windows</TargetFramework>')) 'Target framework must match Revit 2026.5 runtime.'
Assert-True ($project.Contains('$(RevitInstallDir)\RevitAPI.dll')) 'Build must reference the installed Revit API.'
Assert-True ($manifest.Contains('\net10.0-windows\RebarStair.dll')) 'Add-in manifest must load the net10 build.'
Assert-True ($view.Contains('IsDesignTimeCreatable=False')) 'Revit-dependent view model must not be instantiated by the XAML designer.'
Assert-True (-not $viewModel.Contains('/RebarStair;component/Images/')) 'Stair previews must not use packaged images.'
Assert-True ($previewSession.Contains('new PreviewControl(_document, ViewId)')) 'Selected model must use Revit PreviewControl.'
Assert-True ($previewSession.Contains('DisplayStyle.Wireframe')) 'The 3D preview must use wireframe mode.'
Assert-True ($command.Contains('new TransactionGroup(doc, "Create stair rebar")')) 'Preview changes must be grouped for commit or rollback.'
Assert-True ($command.Contains('group.Assimilate()') -and $command.Contains('group.RollBack()')) 'Preview must commit only after confirmation.'
Assert-True ($command.Contains('if (vm.IsPreviewRequested)') -and $command.Contains('vm.CreateRebarPreview()')) 'Modal dialog must close before Revit point selection starts.'
Assert-True (-not $viewModel.Contains('CurrentWindow?.Hide()')) 'A modal WPF window must not hide while Revit selection is requested.'
Assert-True ($view.Contains('Command="{Binding PreviewCommand}"') -and $view.Contains('IsEnabled="{Binding HasPreview}"')) 'Preview and confirmation controls are not wired.'
Assert-True ($command.Contains('GeometryInspector.Inspect(doc, stair)')) 'Selected model must be logged before preview.'
Assert-True ($selectionFilter.Contains('CategoryType.Model')) 'Third-party model elements must be selectable.'
Assert-True ($previewSession.Contains('SetProjectionLineColor') -and $previewSession.Contains('SetProjectionLineWeight')) 'Selected rebar must be outlined in red in the preview.'
Assert-True ($previewSession.Contains('HighlightRebars')) 'Preview session must expose rebar highlighting.'
Assert-True ($takeoff.Contains('HighlightRebars') -and $takeoff.Contains('ApplyRebarHighlight')) 'Rebar selection must drive the preview highlight.'
Assert-True ($viewModel.Contains('SetWorkPlaneSection') -and $viewModel.Contains('ExecuteBooleanOperation')) 'The 2D section must be cut from the picked work plane.'
Assert-True ($viewModel.Contains('SetWorkPlaneSection(planarFace)')) 'Pick WorkPlane must feed the 2D section.'
Assert-True ($view.Contains('Text="Mặt cắt"') -and $view.Contains('Converter={StaticResource SectionGeometry}')) 'The window must draw the 2D section from the section DTO.'
Assert-True (-not $view.Contains('RebarTakeoff') -and -not $viewModel.Contains('RebarTakeoff')) 'The rebar takeoff table must be removed.'
Assert-True (-not $viewModel.Contains('TryApplySavedPoints') -and -not $view.Contains('UseSavedPoints')) 'Saved point templates must be removed; points are picked every time.'
Assert-True ($viewModel.Contains('_services.Points.Select(StairModel, SelectedType.Name)')) 'Rebar placement must ask for the points through the point selection service.'
Assert-True ($viewModel.Contains('RegisterRebar(')) 'Created rebars must be tracked per bar number.'
foreach ($inspectionMarker in @('RebarHostData.IsValidHost', 'GeometryInstance', 'case Solid', 'case Mesh', 'geometry-inspection.log')) {
    Assert-True ($geometryInspector.Contains($inspectionMarker)) "Geometry inspector is missing $inspectionMarker."
}
foreach ($stairType in @('Type1', 'Type2', 'Type3', 'Chiếu Nghỉ')) {
    Assert-True ($viewModel.Contains('Name = "' + $stairType + '"')) "Missing rebar option for $stairType."
}

Assert-True (-not $option.Contains('static event Action<RebarOptions>')) 'Rebar selection event must not be static.'
Assert-True (-not $parameterSetter.Contains('LookupParameter("Partition")')) 'Partition lookup must use its built-in parameter id.'
Assert-True (-not $viewModel.Contains('RebarHookOrientation')) 'Deprecated Revit 2026 hook orientation API must not return.'
Assert-True ($viewModel.Contains('new BarTerminationsData(_document)')) 'Rebar creation must use the Revit 2026 terminations API.'

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

Write-Output "Regression checks passed: native Revit preview lifecycle and $($actualPlans.Count) partition batches."
