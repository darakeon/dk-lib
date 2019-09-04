del *.nupkg

cd Source
cd DK.MVC
nuget pack
move Keon.MVC.*.nupkg ..\..

cd ..
cd DK.NHibernate
nuget pack
move Keon.NHibernate.*.nupkg ..\..

cd ..
cd DK.TwoFactorAuth
nuget pack
move Keon.TwoFactorAuth.*.nupkg ..\..

cd ..
cd DK.Util
nuget pack
move Keon.Util.*.nupkg ..\..

cd ..
cd DK.XML
nuget pack
move Keon.XML.*.nupkg ..\..

cd ..\..
