chcp 65001
@echo off
.\tabtoy -mode=v3 -index="..\Index.xlsx" -proto_out="..\..\ProtoTool\protocols\datatable\PbDataTable.proto" -package="ConfigPB"
cd ..\..\ProtoTool
call GenClientCsharp.bat

pause