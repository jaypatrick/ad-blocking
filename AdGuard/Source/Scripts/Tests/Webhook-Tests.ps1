BeforeAll {
    Import-Module ..\a
}

Describe "Get-Emoji" {
    Context "Lookup by whole name" {
        It "Returns <expected> (<name>)" -TestCases @(
            @{ Name = "cactus"; Expected = '🌵' }
            @{ Name = "giraffe"; Expected = '🦒' }
        ) {
            Get-Emoji -Name $name | Should -Be $expected
        }
    }
}