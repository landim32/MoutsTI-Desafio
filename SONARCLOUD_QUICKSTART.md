# ?? Quick Start - GitHub Actions + SonarCloud

## ? Checklist de Configuração

### 1?? Configurar Secret no GitHub (OBRIGATÓRIO)

1. Acesse: https://github.com/landim32/MoutsTI-Desafio/settings/secrets/actions
2. Clique em **"New repository secret"**
3. Preencha:
   - **Name**: `SONAR_TOKEN`
   - **Secret**: `8a5afa0b227123427cbeb29ce22814d7705a7606`
4. Clique em **"Add secret"**

### 2?? Fazer commit e push dos novos arquivos

```bash
git add .
git commit -m "ci: add SonarCloud integration with GitHub Actions"
git push origin main
```

### 3?? Verificar a execução da pipeline

1. Acesse: https://github.com/landim32/MoutsTI-Desafio/actions
2. Você verá a workflow "SonarCloud Analysis" rodando
3. Aguarde a conclusão (leva ~5-10 minutos)

### 4?? Ver resultados no SonarCloud

Acesse: https://sonarcloud.io/project/overview?id=landim32_MoutsTI-Desafio

---

## ?? Arquivos Criados

? `.github/workflows/sonarcloud.yml` - Pipeline do GitHub Actions  
? `.github/SONARCLOUD_SETUP.md` - Documentação completa  
? `sonar-project.properties` - Configuração do SonarCloud  
? `README.md` - Atualizado com badges e documentação  
? `.gitignore` - Atualizado para ignorar arquivos do SonarQube  

---

## ?? O que acontece automaticamente?

Toda vez que você fizer:
- ? **Push** para `main` ou `develop`
- ? **Pull Request** aberto/atualizado

A pipeline irá:
1. ? Compilar o código
2. ? Executar todos os testes
3. ? Gerar relatório de cobertura
4. ? Enviar análise para o SonarCloud
5. ? Exibir badges no README

---

## ?? Badges no README

Após a primeira execução, os badges mostrarão:
- ?? Quality Gate (passou/falhou)
- ?? Coverage (% de cobertura)
- ?? Bugs (quantidade)
- ?? Code Smells (quantidade)
- ?? Vulnerabilities (quantidade)

---

## ??? Executar localmente (opcional)

```powershell
.\sonar-analysis.ps1
```

---

## ?? Links Úteis

- **GitHub Actions**: https://github.com/landim32/MoutsTI-Desafio/actions
- **SonarCloud Dashboard**: https://sonarcloud.io/project/overview?id=landim32_MoutsTI-Desafio
- **Documentação Completa**: `.github/SONARCLOUD_SETUP.md`

---

## ?? IMPORTANTE

**REMOVA O TOKEN DO ARQUIVO `sonar-analysis.ps1`** antes de fazer commit!

O token deve estar apenas como secret no GitHub, nunca no código.

---

## ?? Pronto!

Agora toda contribuição ao projeto será automaticamente analisada pelo SonarCloud! ??
