# ExtractCert

A tool to export or view certificates, from web sites or tcp services, like LDAPS.

```Shell
-----------------------------------------------------------------------------
Usage: ExtractCert.exe <-web|-tcp> <-h hostname> <-p port> [<-view>|<-certpath c:\temp\cert.cer>] [-savechain]

Save mode:
ExtractCert.exe -web -h www.microsoft.com -p 443 -certpath c:\temp\microsoft.cer
ExtractCert.exe -tcp -h dc1.contoso.com -p 636 -certpath c:\temp\ldaps.cer

ExtractCert.exe -web -h www.microsoft.com -p 443 -certpath c:\temp\microsoft.cer -savechain
ExtractCert.exe -tcp -h dc1.contoso.com -p 636 -certpath c:\temp\ldaps.cer -savechain

View mode:
ExtractCert.exe -web -h www.microsoft.com -p 443 -view
ExtractCert.exe -tcp -h dc1.contoso.com -p 636 -view
-----------------------------------------------------------------------------
```
Sample:

```Shell
C:\>ExtractCert.exe -view -web -h www.microsoft.com -p 443
Subject: CN=www.microsoft.com, OU=Microsoft Corporation, O=Microsoft Corporation, L=Redmond, S=WA, C=US
Subject Alternative Name:

DNS Name=wwwqa.microsoft.com
DNS Name=www.microsoft.com
DNS Name=staticview.microsoft.com
DNS Name=i.s-microsoft.com
DNS Name=microsoft.com
DNS Name=c.s-microsoft.com
DNS Name=privacy.microsoft.com

Issuer: CN=Microsoft IT TLS CA 5, OU=Microsoft IT, O=Microsoft Corporation, L=Redmond, S=Washington, C=US
Thumbprint: 28B88504F609F685F168B9A49C8F0EC49EAD8BC2
Serial#: 2D000C371562C41D9394087F680000000C3715
Valid From: 10/21/2019 7:04:04 PM
Valid To: 10/21/2021 7:04:04 PM
Signature Algorithm: sha256RSA

Extensions:

---> 1.3.6.1.4.1.11129.2.4.2 SCT List
v1
f65c942fd1773022145418083094568ee34d131933bfdf0c2f200bcc4ef164e3
?Monday, ?October ?21, ?2019 7:14:06 PM
SHA256
ECDSA
304502210095d283ce168b4c9d4fa53a44ed39ab469fa70f0a0465b5b924e7eb3caa11e8fb022013aa73a6f3bbb3277deac16f19142b04baecba454c9831624e9157df26e5f2b6

v1
5581d4c2169036014aea0b9b573c53f0c0e43878702508172fa3aa1d0713d30c
?Monday, ?October ?21, ?2019 7:14:06 PM
SHA256
ECDSA
304402205a26a1c662b93eca1ddc72e7a5b09c6a2db844dc326e4816717bec44a6269b85022045d71c921b7fc7cb86ead4ab44a839d067709d694528c0434f9d2b42320f7e53

v1
7d3ef2f88fff88556824c2c0ca9e5289792bc50e78097f2e6a9768997e22f0d7
?Monday, ?October ?21, ?2019 7:14:06 PM
SHA256
ECDSA
3044022064e8a97a25bc74deb357f443057d452bc41391d5c17ea39fbd3e29a131974f81022068b0e6de1d253cebb14ed40c39770f91d35659d5fb9d1f10f0b9780bbea16643

---> 1.3.6.1.4.1.311.21.10 Application Policies
[1]Application Certificate Policy:
     Policy Identifier=Client Authentication
[2]Application Certificate Policy:
     Policy Identifier=Server Authentication

---> 1.3.6.1.4.1.311.21.7 Certificate Template Information
Template=1.3.6.1.4.1.311.21.8.16155509.8105089.5391003.2969441.12400096.221.9744322.5884410
Major Version Number=100
Minor Version Number=29

---> 1.3.6.1.5.5.7.1.1 Authority Information Access
[1]Authority Info Access
     Access Method=Certification Authority Issuer (1.3.6.1.5.5.7.48.2)
     Alternative Name:
          URL=http://www.microsoft.com/pki/mscorp/Microsoft IT TLS CA 5.crt (http://www.microsoft.com/pki/mscorp/Microsoft%20IT%20TLS%20CA%205.crt)
[2]Authority Info Access
     Access Method=On-line Certificate Status Protocol (1.3.6.1.5.5.7.48.1)
     Alternative Name:
          URL=http://ocsp.msocsp.com

---> 2.5.29.14 Subject Key Identifier
f6abbf051e41b770e991f81a956ef60c2b09fb95

---> 2.5.29.15 Key Usage
Digital Signature, Key Encipherment, Data Encipherment (b0)

---> 2.5.29.31 CRL Distribution Points
[1]CRL Distribution Point
     Distribution Point Name:
          Full Name:
               URL=http://mscrl.microsoft.com/pki/mscorp/crl/Microsoft IT TLS CA 5.crl (http://mscrl.microsoft.com/pki/mscorp/crl/Microsoft%20IT%20TLS%20CA%205.crl)
               URL=http://crl.microsoft.com/pki/mscorp/crl/Microsoft IT TLS CA 5.crl (http://crl.microsoft.com/pki/mscorp/crl/Microsoft%20IT%20TLS%20CA%205.crl)

---> 2.5.29.32 Certificate Policies
[1]Certificate Policy:
     Policy Identifier=1.3.6.1.4.1.311.42.1
     [1,1]Policy Qualifier Info:
          Policy Qualifier Id=CPS
          Qualifier:
               http://www.microsoft.com/pki/mscorp/cps

---> 2.5.29.35 Authority Key Identifier
KeyID=08fe259f74ea8704c2bcbb8ea8385f33c6d16c65

---> 2.5.29.37 Enhanced Key Usage
Client Authentication (1.3.6.1.5.5.7.3.2)
Server Authentication (1.3.6.1.5.5.7.3.1)

---> Cert hierarchy (chain):
        Subject 1: CN=www.microsoft.com, OU=Microsoft Corporation, O=Microsoft Corporation, L=Redmond, S=WA, C=US
        Subject 2: CN=Microsoft IT TLS CA 5, OU=Microsoft IT, O=Microsoft Corporation, L=Redmond, S=Washington, C=US
        Subject 3: CN=Baltimore CyberTrust Root, OU=CyberTrust, O=Baltimore, C=IE


-----------------------------------------------------------------------------
```
Sample:

```Shell
C:\>ExtractCert.exe -view -tcp -h dc1.contoso.com -p 636
Subject: CN=DC1.contoso.com
Subject Alternative Name:

Other Name:
     DS Object Guid=04 10 bc 93 45 18 f4 ba cf 4e a2 4b ae 0d 5b 76 1d 01
DNS Name=DC1.contoso.com

Issuer: CN=Contoso-CA, DC=contoso, DC=com
Thumbprint: A43092FDAFA0150820D8F9D470CC8D132A332243
Serial#: 7700000003D994208DE4E6788A000000000003
Valid From: 10/9/2019 2:48:47 PM
Valid To: 10/8/2020 2:48:47 PM
Signature Algorithm: sha256RSA

Extensions:

---> 1.3.6.1.4.1.311.20.2 Certificate Template Name
DomainController

---> 2.5.29.37 Enhanced Key Usage
Client Authentication (1.3.6.1.5.5.7.3.2)
Server Authentication (1.3.6.1.5.5.7.3.1)

---> 2.5.29.15 Key Usage
Digital Signature, Key Encipherment (a0)

---> 1.2.840.113549.1.9.15 SMIME Capabilities
[1]SMIME Capability
     Object ID=1.2.840.113549.3.2
     Parameters=02 02 00 80
[2]SMIME Capability
     Object ID=1.2.840.113549.3.4
     Parameters=02 02 00 80
[3]SMIME Capability
     Object ID=2.16.840.1.101.3.4.1.42
[4]SMIME Capability
     Object ID=2.16.840.1.101.3.4.1.45
[5]SMIME Capability
     Object ID=2.16.840.1.101.3.4.1.2
[6]SMIME Capability
     Object ID=2.16.840.1.101.3.4.1.5
[7]SMIME Capability
     Object ID=1.3.14.3.2.7
[8]SMIME Capability
     Object ID=1.2.840.113549.3.7

---> 2.5.29.14 Subject Key Identifier
0fb1feabac0bbc5dd2c45e7a8e81646cc8744e52

---> 2.5.29.35 Authority Key Identifier
KeyID=e4503441423ffb0618d645e46e1f32ea474c99ba

---> 2.5.29.31 CRL Distribution Points
[1]CRL Distribution Point
     Distribution Point Name:
          Full Name:
               URL=ldap:///CN=Contoso-CA,CN=DC1,CN=CDP,CN=Public Key Services,CN=Services,CN=Configuration,DC=contoso,DC=com?certificateRevocationList?base?objectClass=cRLDistributionPoint (ldap:///CN=Contoso-CA,CN=DC1,CN=CDP,CN=Public%20Key%20Services,CN=Services,CN=Configuration,DC=contoso,DC=com?certificateRevocationList?base?objectClass=cRLDistributionPoint)

---> 1.3.6.1.5.5.7.1.1 Authority Information Access
[1]Authority Info Access
     Access Method=Certification Authority Issuer (1.3.6.1.5.5.7.48.2)
     Alternative Name:
          URL=ldap:///CN=Contoso-CA,CN=AIA,CN=Public Key Services,CN=Services,CN=Configuration,DC=contoso,DC=com?cACertificate?base?objectClass=certificationAuthority (ldap:///CN=Contoso-CA,CN=AIA,CN=Public%20Key%20Services,CN=Services,CN=Configuration,DC=contoso,DC=com?cACertificate?base?objectClass=certificationAuthority)

---> Cert hierarchy (chain):
        Subject 1: CN=DC1.contoso.com
        Subject 2: CN=Contoso-CA, DC=contoso, DC=com
```