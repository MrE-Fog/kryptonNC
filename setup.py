from ast import arg
from setuptools import setup
from setuptools.command.install import install
from pybind11.setup_helpers import Pybind11Extension
import os
import sys

with open("README.md","r") as file:
  description=file.read()

class opensslFipsValidate(install):
  def run(self):
    install.run(self)
    openssl_fips_module = "openssl-install/lib/ossl-modules/fips.dll" if sys.platform == "win32" else "openssl-install/lib/ossl-modules/fips.so" 
    openssl_fips_conf = "pysec/fipsmodule.cnf"
    temp = os.getcwd()
    os.chdir(os.path.join(self.install_base,"Lib\\site-packages"))
    open(openssl_fips_conf,"w").close()
    print("Running self-tests for openssl fips validated module")
    os.system('"openssl-install\\bin\\openssl" fipsinstall -out {openssl_fips_conf} -module {openssl_fips_module}'
      .format(openssl_fips_module=openssl_fips_module,openssl_fips_conf=openssl_fips_conf))
    os.chdir(temp)


  
setup(name='pysec',
  version='1.0',
  description='pysec',
  long_description=description,
  long_description_content_type="text/markdown",
  author='Mark Barsi-Siminszky',
  author_email='mark.barsisiminszky@outlook.com',
  url='https://github.com/mbs9org/PySec',
  project_urls={
    "Bug Tracker": "https://github.com/mbs9org/PySec/issues",
  },
  classifiers=[
      "License :: OSI Approved :: Apache Software License",
      "Operating System :: OS Independent",
  ],
  package_data={"":["../openssl-install/bin/libcrypto-3-x64.dll",
    "../openssl-install/lib/ossl-modules/fips.dll",
    "../openssl-install/bin/openssl.exe"]},
  packages=['pysec'],
  python_requires=">3.8",
  include_package_data=True,
  cmdclass={
    'install': opensslFipsValidate
  },
  ext_modules=[Pybind11Extension('CryptoLib', 
    ['CryptoLib/Cryptolib.cpp'], 
    include_dirs=["openssl/include","CryptoLib"],
    library_dirs=["openssl"],
    libraries=["libcrypto"])]
)
