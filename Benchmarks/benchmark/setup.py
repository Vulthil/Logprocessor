from setuptools import setup, find_packages


#with open('../README.md') as f:
#    readme = f.read()

#with open('LICENSE') as f:
#    license = f.read()

REQS = [
    "docker",
    "requests",
    "defaultlist"
]

setup(
    name='benchmark',
    version='0.1.0',
    description='Benchmark package for Thesis',
    #long_description=readme,
    author='Alexander Østergaard & Jens Kornbeck',
    author_email='s164424@student.dtu.dk;s164434@studennt.dtu.dk',
    url='',
    install_requires=REQS,
    ##license=license,
    packages=find_packages(exclude=('tests', 'docs'))
)