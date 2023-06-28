
echo Verifica se existe uma imagem Docker existente e a exclui
docker image rm unifenashackaton

echo Executa o comando para criar a imagem Docker
docker build -t unifenashackaton .
pause
cls
echo Executa o comando para fazer login no Heroku Container Registry
heroku container:login

cls

echo Executa o comando para fazer o push da imagem para o Heroku
heroku container:push web -a unifenashackaton
cls

echo Executa o comando para liberar a imagem no Heroku
heroku container:release web -a unifenashackaton
