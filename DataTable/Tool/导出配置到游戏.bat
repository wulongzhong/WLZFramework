chcp 65001
@echo off
.\tabtoy -mode=v3 -index="..\Index.xlsx" -json_out="..\..\Assets\GamePlay\DataTable\all.txt"
.\tabtoy -mode=v3 -index="..\Index.xlsx" -pbbin_out="..\..\Assets\GamePlay\DataTable\all.bytes"

pause