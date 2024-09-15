from os import system
from subprocess import PIPE, run
from re import search, sub
from sys import argv


def terminal(command):
    process = run(command.split(' '), stdout=PIPE)
    return process.stdout.decode('utf-8').split('\n')


commits = terminal('git log origin/main.. --pretty=format:%s')
diff_files = terminal('git diff origin/main --stat')

commit_acronyms = {
    'Eml': 'eml',
    'MVC': 'mvc',
    'NHibernate': 'nh',
    'TwoFactorAuth': 'tfa',
    'Util': 'util',
    'XML': 'xml',
}

for diff_file in diff_files:
    if '.csproj' not in diff_file:
        continue

    file_path = diff_file.split(' ')[1]
    project = file_path.split('/')[1]

    if project.endswith('.Tests'):
        continue

    acronym = commit_acronyms[project]

    commit_message = f'{acronym}: update version'

    if commit_message in commits:
        print(f'Already committed for project {project}')

    else:
        with open(file_path) as file:
            content = file.read()

        pattern = r'(<Version>\d+\.\d+\.)(\d+)(</Version>)'
        version = search(pattern, content)

        current = int(version.group(2))
        new = current + 1
        content = sub(pattern, rf'\g<1>{new}\g<3>', content)

        with open(file_path, 'w') as file:
            file.write(content)

        with open('file_path', 'w') as file:
            file.write(file_path)

        with open('commit_message', 'w') as file:
            file.write(commit_message)
