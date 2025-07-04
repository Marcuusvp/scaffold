Iniciando o projeto
1. rodar o docker-compose -d
2. acessar o endereço onde subimos o kibana -> localhost:5601
3. registrar manual o endereço do compose na interface do kibana http://elastichsearch:9200
4. Quando ele pedir um código, esse código vai aparecer nos logs do container do kibana
5. Para configurar o elastic temos esse video tutorial, parte final dele é toda na interface https://www.youtube.com/watch?v=B79IOUz77xo&list=WL&ab_channel=VasiliiOleinic

Atualização
6. O projeto possui um .env com as configs para enviar os logs/traces para o grafana cloud. Os logs seguem sendo enviados ao kibana também.
7. Agora temos uma entidade com propriedades e endpoints para testar inserts. basta rodar os comandos
->  dotnet ef migrations add InitialCreate
->  dotnet ef database update 
8. Após rodar, basta testar o endpoint de insert com
{
  "level": "Information",
  "message": "Teste de log",
  "details": "Detalhes adicionais aqui"
}
9. Checar os logs no grafana, em drilldown -> Logs