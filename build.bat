msbuild /t:Rebuild /p:configuration=Release ElasticSearch.client.sln
.\tools\ilmerge.exe /internalize /target:library /out:.\build\ES.dll .\ElasticSearch.Client\bin\Release\ElasticSearch.Client.dll .\ElasticSearch.Client\bin\Release\Json.Net.3.5.dll .\ElasticSearch.Client\bin\Release\log4net.dll .\ElasticSearch.Client\bin\Release\Lucene.Net.2.9.dll .\ElasticSearch.Client\bin\Release\Microsoft.Contracts.dll .\ElasticSearch.Client\bin\Release\NetReflector.dll