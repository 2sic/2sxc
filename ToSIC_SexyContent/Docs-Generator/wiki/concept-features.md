# Concept: Features _new in 9.30_

To increase the security of 2sxc, many features are [only available if actively enabled](https://2sxc.org/en/blog/post/new-features-management-in-2sxc-9-30). This [reduced the security surface and hardens the installation](https://2sxc.org/en/blog/post/new-features-management-in-2sxc-9-30).

A feature is a functionality of 2sxc or EAV, which can be enabled/disabled at system level. Each feature is identified by a GUID, and all features are disabled by default. Some examples of features:

1. define permissions by user (_new in 9.30_)
1. use the new (beta) Angular 5 UI for editing (_new in 9.30_)
1. use standard 2sxc forms to allow public users to submit data (_new in 9.30_)
1. let certain users save draft-only data (_new in 9.30_)
1. enable paste-image-from-clipboard (beta, _new in 9.30_)

## Behavior if Feature is Disabled

If a feature is not enabled and code needs to use the feature, it will either show an error OR simply skip that functionality. This varies from feature to feature. If an error is thrown, the system will include a link to the missing feature as well as instructions to enable.

## Managing Features

This is done through the Apps-Management. This is what it looks like:

<img src="assets/concepts/features-manage.png" width="100%">

Read about managing features in the [blog about features-management](https://2sxc.org/en/blog/post/new-features-management-in-2sxc-9-30)

## How the Feature-Configurations are Stored

Features use a json-file called `features.json` located in the `desktopmodules/tosic_sexycontent/.data-custom/configurations` folder.

## Additional Security Mechanisms

For added security, there are two layers of additional protection:

1. The configuration contains a fingerprint of the current installation (so that an attacker cannot simply replace the features-configuration with an own copy). This fingerprint should match the fingerprint of the installation for the configuration to be valid. Read more about [fingerprinting in this blog](https://2sxc.org/en/blog/post/system-fingerprint-used-in-2sxc-9-30).
1. When the configuration is created, it is signed by the 2sxc.org server using a digital certificate and verified when the features are loaded.

_Note: the security features are already built into 9.30 but not yet enforced. This is because it's a fairly new setup, and we want to be sure that we don't accidentally disable something if something doesn't work properly._


## Read also

* [Blog about new security mechanisms in 9.30](https://2sxc.org/en/blog/post/security-first-strategy-for-2sxc-9-30)


## History

1. Feature system introduced in 2sxc 9.30