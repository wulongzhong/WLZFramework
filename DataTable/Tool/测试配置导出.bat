chcp 65001
@echo off
.\tabtoy -mode=v3 -index="..\Index.xlsx" -json_out="test.txt"
.\tabtoy -mode=v3 -index="..\Index.xlsx" -pbbin_out="test.bytes"

pause