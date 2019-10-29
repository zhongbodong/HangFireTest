FROM registry-vpc.cn-shenzhen.aliyuncs.com/mytools/dotnetsdk:2.2-runtime

RUN mkdir /Fairhr.Jobs

COPY job-release /Fairhr.Jobs

WORKDIR /Fairhr.Jobs

EXPOSE 80

ENV ASPNETCORE_ENVIRONMENT=Production

CMD ["dotnet", "Fairhr.Jobs.dll"]
