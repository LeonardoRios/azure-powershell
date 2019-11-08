$TestRecordingFile = Join-Path $PSScriptRoot 'Restart-AzFunctionApp.Recording.json'
$currentPath = $PSScriptRoot
while(-not $mockingPath) {
    $mockingPath = Get-ChildItem -Path $currentPath -Recurse -Include 'HttpPipelineMocking.ps1' -File
    $currentPath = Split-Path -Path $currentPath -Parent
}
. ($mockingPath | Select-Object -First 1).FullName

Describe 'Restart-AzFunctionApp' {
    It 'Restart' -skip {
        { throw [System.NotImplementedException] } | Should -Not -Throw
    }

    It 'ByObjectInput' -skip {
        { throw [System.NotImplementedException] } | Should -Not -Throw
    }
}
