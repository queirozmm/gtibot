IMPORTANTE
==========

* No arquivo application.json, defina os valores corretos para:
	- identifier
	- accessKey
	- settings/mpa/WalletId
	- settings/mpa/SessionIdentifier
	- settings/talkService/Context (se utilizar o TalkService)

* Defina a pasta dos logs em disco no NLog.config

* Métricas do seu bot serão salvas na tabela 'Metrics', na base especificada na connection string do 'application.json'. 
São incluidas por padrão: 'Requests', timer que contabiliza mensagens processadas por segundo e 'ConcurrentRequests', que conta mensagens processadas simultaneamente;
MpaRequests, timer que contabiliza requisições ao MPA por segundo, MpaErrorRequests, que conta requisições com erro, e MpaConcurrentRequests, que conta requisições simultâneas.

* Defina a porta para o 'report' HTTP das métricas no application.json, na chave 'httpEndpointPort'.

* Ao criar a wallet no MPA, cadastrar as seguintes palavras chaves e tratá-las:

        //Primeira interação com o Bot
        "#ATIVAR"
		"#WELCOME"
		 
		//Tratamentos de Mídia
		"#ERRORAUDIO"
        "#ERRORVIDEO"
        "#ERRORIMAGE"
        "#ERRORFILE"
        "#MIDIANAORECONHECIDA"

		//NLP
		 "#DONTKNOWANSWER"
         "#INICIO"

		 //Erro Genérico para qualquer problema que ocorra no bot
         "#ErroGenerico"
DICA
====
É seguro remover este README do projeto e também remover do arquivo packages.config as referências (editar diretamente o 
arquivo e remover as respectivas linhas) das bibliotecas
'Takenet.MessagingHub.Client.CustomerSuccess.Template' e 'Takenet.MessagingHub.Client.Template'