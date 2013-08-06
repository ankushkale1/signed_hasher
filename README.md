signed_hasher
=============

File Hash program with IconOverlays &amp; PropertySheet.

Features:

1. Uses SHA256 hash algorithm.
2. Provides IconOverlays to provide at-a-glance view of File Health. ( provides check icon on File if hash ok ).
3. Provices Property Sheets ( a new tab in file/folder properties ) for easy create/check file hashes.
4. Sign's hash files to detect tampering of hash files.
5. Automatically Uploads File Hashes to Google Drive for Second Verification ( If Private key is compromised ).
6. You can add your download folder to watch for new files. Hasher automatically prompts to sign file.
7. Filtered hashing so that only needed files (eg. exe,msi) will be hashed.

HowTo:

1. You first need GDrive/Google Drive account.
2. Then Create API key, Write down ClientID & ClientSecret strings.
3. Now Click on Show Settings. Here you can select previously generated .PFX file ( select .PFX file & enter password ) or generate new certificate( just enter password in password textbox & click on Create New Button)
4. Now click on Browse to select folder
5. optional: Add filter eg.*.exe,*.msi,*.dll
6. Click on Generate/Check.

This is Fun project. You can contact me at: ankushkale1[at]gmail[dot]com.
